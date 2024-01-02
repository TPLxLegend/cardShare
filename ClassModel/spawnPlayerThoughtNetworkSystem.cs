using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class spawnPlayerSystem : SingletonNetworkPersistent<spawnPlayerSystem>
{
    [SerializeField] GameObject CameraPre;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] List<GameObject> characterPrefab;
    [SerializeField] GameObject team;

    NetworkObject netSpawned;
    [SerializeField] GameObject namePlayerCanvas;

    [ServerRpc(RequireOwnership = false)]
    public void spawnPlayerServerRpc(Vector3 pos, Quaternion rot, ulong clientID)
    {
        Debug.Log("pos when call rpc:   " + pos);
        Debug.Log("who call it: " + clientID);
        var newSpawned = Instantiate(playerPrefab, pos +new Vector3(Random.Range(-3f,3f), 0f, Random.Range(-3f, 3f)), rot);   //+new Vector3(clientID,0,0)
        netSpawned = newSpawned.GetComponent<NetworkObject>();
        netSpawned.SpawnWithOwnership(clientID);
        Debug.Log(netSpawned + " is own by client: " + netSpawned.OwnerClientId);

        //spawn Team empty
        var teamIns = Instantiate(team);
        var teamNet = teamIns.GetComponent<NetworkObject>();
        teamNet.SpawnWithOwnership(clientID);
        teamNet.TrySetParent(netSpawned.transform, false);
        

        //spawn first character into Team 
        var teamNetRef = new NetworkObjectReference(teamNet);
        spawnCharacterIntoServerRpc(clientID, 0, teamNetRef);


        ClientRpcParams clientRpcParams = new ClientRpcParams()
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };
        //ref
        NetworkObjectReference netObjRef = new NetworkObjectReference(netSpawned);
        var playerInfo = teamNet.GetComponentInChildren<playerInfo>();
        Debug.Log("info before call client:" + playerInfo);
        NetworkBehaviourReference infoRef = new NetworkBehaviourReference(playerInfo);

        //call client 
        setPlayerClientRpc(netObjRef, clientID, clientRpcParams);
        initInforCanvasOnOtherClientRpc(clientID, netObjRef, infoRef);
        asignInfoToOtherClientRpc(clientID, infoRef);
    }

    [ServerRpc(RequireOwnership = false)]
    public void spawnCharacterIntoServerRpc(ulong clientID, int index, NetworkObjectReference teamRef)
    {
        teamRef.TryGet(out NetworkObject teamNet);
        Transform team = teamNet.transform;
        var charIns = Instantiate(characterPrefab[index]);
        var insNet = charIns.GetComponent<NetworkObject>();
        insNet.SpawnWithOwnership(clientID, team);


        bool res = insNet.TrySetParent(team, false);
        Debug.Log("res of spawn character " + characterPrefab[index] + " of client " + NetworkBehaviourId + ":   " + res);
    }

    [ClientRpc]
    public void setPlayerClientRpc(NetworkObjectReference networkObjectRef, ulong clientId = 0, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("server call to set player" + clientRpcParams.Receive.ToString());
        //clear when 
        if (NetworkManager.Singleton.LocalClientId != clientId)
        {
            Debug.Log("server send a boardcard when use this clientRPC");
            return;
        }
        var playerControll = PlayerController.Instance;

        if (networkObjectRef.TryGet(out NetworkObject networkObj))
        {
            Debug.Log(networkObj.gameObject + " have ownship by: " + networkObj.OwnerClientId);
            playerControll.player = networkObj.gameObject;
        }
        var tf = networkObj.gameObject.transform;
        Instantiate(CameraPre, tf.position, tf.rotation);

        var controllRec = playerControll.controllReceivingSystem = networkObj.gameObject.GetComponent<ControllReceivingSystem>();
        controllRec.ReLoadCurCharacter();
        controllRec.onCurCharacterChange.AddListener(playerControll.loadPlayerInfo);



        playerControll.controller = controllRec.characterController;
        playerControll.loadPlayerInfo(controllRec.curCharacterControl);
    }
    #region UI
    [ClientRpc]
    public void initInforCanvasOnOtherClientRpc(ulong clientId, NetworkObjectReference NetRef, NetworkBehaviourReference plInfoRef)
    {
        plInfoRef.TryGet(out playerInfo info);
        NetRef.TryGet(out NetworkObject Net);
        string plname = playerGeneralInfo.Instance.namePlayer = "Người chơi " + clientId;
        GameObject infoCanvas = Instantiate(namePlayerCanvas, Net.transform);
        infoCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        infoCanvas.GetComponentInChildren<TMPro.TMP_Text>().text = plname;
        infoCanvas.AddComponent<alwayFaceCamera>();

        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            var otherPlayerHpUI = infoCanvas.AddComponent<hpBarInMap>();
            info.hp.OnValueChanged += (o, n) =>
            {
                otherPlayerHpUI.syncValue(n / (float)info.maxHP);
            };
        }
    }
    [ClientRpc]
    public void asignInfoToOtherClientRpc(ulong clientID, NetworkBehaviourReference playerInfoRef)
    {
       // if (NetworkManager.LocalClientId == clientID) return;
        playerInfoRef.TryGet(out playerInfo info);
        Debug.Log("info after call client:" + info);
        otherPlayerInfo.Instance.SpawnInfo(info, clientID);
    }
    #endregion
    #region bullet
    public GameObject bulletVFX;
    [ServerRpc(RequireOwnership = false)]
    public void spawnBulletServerRpc(ulong clientID, float bulletSpeed, Vector3 pos, Quaternion rot)
    {
        Debug.Log("spawn bullet server rpc called");
        spawnBulletClientRpc(clientID, bulletSpeed, pos, rot);
    }
    [ClientRpc]
    public void spawnBulletClientRpc(ulong clientID, float bulletSpeed, Vector3 pos, Quaternion rot)
    {
        Debug.Log("spawn bullet client rpc called");
        GameObject bullet = Instantiate(bulletVFX, pos, rot);

        bullet.GetComponentInChildren<Rigidbody>().isKinematic = false;

        var direction = bullet.transform.forward;
       
        var vfx = bullet.GetComponent<VisualEffect>();
        GameObject bulletParticle = bullet.transform.GetChild(0).gameObject;
        skillObj bulletScript = bulletParticle.AddComponent<skillObj>();
        bulletScript.onUpdate = new UnityEngine.Events.UnityEvent<skillObj>();
        bulletScript.collisionEnter = new UnityEngine.Events.UnityEvent<GameObject, GameObject>();

        bulletScript.onUpdate.AddListener((self) =>
        {
            self.gameObject.transform.position += direction * bulletSpeed * Time.deltaTime;
        });
        bulletScript.collisionEnter.AddListener((selfGO, collideGO) =>
        {
            if (collideGO.TryGetComponent(out enemyInfo info))
            {
                var plinfo = PlayerController.Instance.playerInfo;

                info.takeDamage(plinfo.attack, DmgType.Physic);
            }
            var vfx = selfGO.GetComponentInParent<VisualEffect>();
            //Debug.Log("vfx of bullet: "+vfx);
            vfx.SendEvent("onExplode");
            Debug.Log("bullet:" + selfGO + "  collide with " + collideGO);
            Destroy(selfGO);
        });
        Destroy(bullet, 20);
    }
    #endregion
    #region cardObject
    [ServerRpc(RequireOwnership = false)]
    public void spawnCardSkillObjectServerRpc(string nameSkillObj, Vector3 pos, Quaternion rot, float duration, skillMoveType skillMoveType,
    targetFilterType detecttype, triggerType triggerType, Vector3 targetPosition, float speed, int timeStandby, Effect[] cardeffect)
    {
        GameObject InsSkillObj = Instantiate(itemPooling.Instance.TakeOut(nameSkillObj), pos, rot);
        skillObj skillObjScript = InsSkillObj.GetComponent<skillObj>();

        //skillObjScript.source = InsSkillObj;
        var skillObjRef = new NetworkBehaviourReference(skillObjScript);
        spawnCardSkillObjectClientRpc(skillObjRef, duration, skillMoveType, detecttype, triggerType, targetPosition, speed, timeStandby, cardeffect);
    }
    [ClientRpc]
    public void spawnCardSkillObjectClientRpc(NetworkBehaviourReference skillObjRef, float duration, skillMoveType skillMoveType,
     targetFilterType detecttype, triggerType triggerType, Vector3 targetPosition, float speed, int timeStandby, Effect[] cardEffect)
    {
        skillObjRef.TryGet(out skillObj skillObj);
        Destroy(skillObj.gameObject, duration);

        Dic.singleton.moveTypes[skillMoveType].addMoveAsync(skillObj.gameObject, targetPosition, speed, timeStandby);
        Dic.singleton.filter[detecttype].addFilterion(skillObj);
        Dic.singleton.trigger[triggerType].addTrigger(skillObj, cardEffect);
    }
    #endregion

    #region netSpawnObj
    [ServerRpc(RequireOwnership =false)]
    public void spawnFromNetManServerRpc(ulong clientId,string namePrefab,Vector3 pos,Quaternion rot)
    {
        GameObject prefab = itemPooling.Instance.getPrefab(namePrefab);
        var go = Instantiate(prefab, pos, rot);
        var netIns = go.GetComponent<NetworkObject>();
        netIns.SpawnWithOwnership(clientId);


    }
    #endregion
}
