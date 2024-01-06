using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
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
        var newSpawned = Instantiate(playerPrefab, pos + new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f)), rot);   //+new Vector3(clientID,0,0)
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
        spawnNameCanvas(clientID, netSpawned, playerInfo);
        // thay the cho asignInfoToOtherClientRpc(clientID, infoRef);
        otherPlayerInfo.Instance.SpawnInfoServerRpc(infoRef, clientID);
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
    public void spawnNameCanvas(ulong clientId, NetworkObject Net, playerInfo plInfo)
    {
        GameObject infoCanvas = Instantiate(namePlayerCanvas);
        string plname = playerGeneralInfo.Instance.namePlayer = "Người chơi " + clientId;
        infoCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        infoCanvas.GetComponentInChildren<TMPro.TMP_Text>().text = plname;
        infoCanvas.gameObject.AddComponent<alwayFaceCamera>();
        var insNet = infoCanvas.GetComponent<NetworkObject>();
        insNet.SpawnWithOwnership(clientId);
        insNet.TrySetParent(Net.transform, false);

        var nameRef = new NetworkObjectReference(insNet);
        var playerInfoRef = new NetworkBehaviourReference(plInfo);
        handleHpBarClientRpc(clientId, nameRef, playerInfoRef);
    }
    [ClientRpc]
    public void handleHpBarClientRpc(ulong clientID, NetworkObjectReference nameRef, NetworkBehaviourReference playerInfoRef)
    {
        nameRef.TryGet(out NetworkObject name);
        playerInfoRef.TryGet(out playerInfo info);
        if (NetworkManager.LocalClientId == clientID && name.transform.childCount > 1)
        {
            name.transform.GetChild(1).gameObject.SetActive(false);
            return;
        }
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

                info.takeDamage(plinfo.attack, DmgType.Electric);
            }
            var vfx = selfGO.GetComponentInParent<VisualEffect>();
            //Debug.Log("vfx of bullet: "+vfx);
            vfx.SendEvent("onExplode");
            Debug.Log("bullet:" + selfGO + "  collide with " + collideGO);
            Destroy(selfGO);
        });
        Destroy(bullet, 10);
    }
    #endregion
    #region cardObject
    [ServerRpc(RequireOwnership = false)]
    public void spawnCardSkillObjectServerRpc(ulong clientID, string nameSkillObj, Vector3 pos, Quaternion rot, float duration, skillMoveType skillMoveType,
    targetFilterType detecttype, triggerType triggerType, Vector3 targetPosition, float speed, int timeStandby, Effect[] cardeffect)
    {
        GameObject InsSkillObj = Instantiate(itemPooling.Instance.getPrefab(nameSkillObj), pos, rot);
        skillObj skillObjScript = InsSkillObj.GetComponent<skillObj>();
        var netObj = InsSkillObj.GetComponent<NetworkObject>();
        netObj.Spawn();

        Dic.singleton.moveTypes[skillMoveType].addMoveAsync(InsSkillObj, targetPosition, speed, timeStandby);

        var skillObjRef = new NetworkBehaviourReference(skillObjScript);
        spawnCardSkillObjectClientRpc(clientID, skillObjRef, duration, detecttype, triggerType, cardeffect);
    }
    [ClientRpc]
    public void spawnCardSkillObjectClientRpc(ulong clientID, NetworkBehaviourReference skillObjRef, float duration,
     targetFilterType detecttype, triggerType triggerType, Effect[] cardEffect)
    {
        if (clientID != NetworkManager.LocalClientId)
        {
            return;
        }
        skillObjRef.TryGet(out skillObj skillObj);

        Dic.singleton.filter[detecttype].addFilterion(skillObj);
        Dic.singleton.trigger[triggerType].addTrigger(skillObj, cardEffect);
        Destroy(skillObj.gameObject, duration);
    }
    #endregion
}
