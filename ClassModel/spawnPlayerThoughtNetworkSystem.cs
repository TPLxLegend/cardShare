using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class spawnPlayerSystem : SingletonNetworkPersistent<spawnPlayerSystem>
{
    [SerializeField] GameObject CameraPre;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] List<GameObject> characterPrefab;
    [SerializeField] GameObject team;

    /// <summary>
    /// it will be change with any serverrpc spawned 
    /// </summary>
    NetworkObject netSpawned;
    [SerializeField] GameObject namePlayerCanvas;

    [ServerRpc(RequireOwnership = false)]
    public void spawnPlayerServerRpc(Vector3 pos, Quaternion rot, ulong clientID)
    {
        Debug.Log("pos when call rpc:   " + pos);
        Debug.Log("who call it: " + clientID);
        var newSpawned = Instantiate(playerPrefab, pos, rot);
        netSpawned = newSpawned.GetComponent<NetworkObject>();
        netSpawned.SpawnWithOwnership(clientID);
        Debug.Log(netSpawned + " is own by client: " + netSpawned.OwnerClientId);

        spawnCharacterIntoServerRpc(clientID, team, netSpawned.transform);
        //netspawned is change to team 
        spawnCharacterIntoServerRpc(clientID, characterPrefab[0], netSpawned.transform);


        ClientRpcParams clientRpcParams = new ClientRpcParams()
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };
        NetworkObjectReference netObjRef = new NetworkObjectReference(netSpawned);
        setPlayerClientRpc(netObjRef, clientID, clientRpcParams);
    }

    [ServerRpc(RequireOwnership = false)]
    public void spawnCharacterIntoServerRpc(ulong clientID, GameObject character, Transform team)
    {
        var charIns = Instantiate(character);
        var insNet = charIns.GetComponent<NetworkObject>();
        insNet.SpawnWithOwnership(clientID, team);


        bool res = insNet.TrySetParent(team, false);
        Debug.Log(res);
        netSpawned = insNet;
    }

    [ClientRpc]
    public void setPlayerClientRpc(NetworkObjectReference networkObject, ulong clientId = 0, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("server call to set player" + clientRpcParams.Receive.ToString());
        //clear when 
        if (NetworkManager.Singleton.LocalClientId != clientId)
        {
            Debug.Log("server send a boardcard when use this clientRPC");
            return;
        }
        var playerControll = PlayerController.Instance;

        if (networkObject.TryGet(out NetworkObject networkObj))
        {
            Debug.Log(networkObj.gameObject + " have ownship by: " + networkObj.OwnerClientId);
            playerControll.player = networkObj.gameObject;
        }
        var tf = networkObj.gameObject.transform;
        networkObj.GetComponent<ControllReceivingSystem>().ReLoadCurCharacter();
        Instantiate(CameraPre, tf.position, tf.rotation);

        var controllRec = playerControll.controllReceivingSystem = networkObj.gameObject.GetComponent<ControllReceivingSystem>();
        controllRec.onCurCharacterChange.AddListener(playerControll.loadPlayerInfo);
        playerControll.controller = controllRec.characterController;
        playerControll.loadPlayerInfo(controllRec.curCharacterControl);
        string plname = playerGeneralInfo.Instance.namePlayer = "Người chơi " + clientId;
        GameObject infoCanvas = Instantiate(namePlayerCanvas, tf);
        infoCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        infoCanvas.GetComponentInChildren<TMPro.TMP_Text>().text = plname;
        infoCanvas.AddComponent<alwayFaceCamera>();
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            var otherPlayerHpUI = infoCanvas.AddComponent<hpBarInMap>();
            playerControll.playerInfo.hp.OnValueChanged += (o, n) =>
            {
                otherPlayerHpUI.syncValue(n / (float)playerControll.playerInfo.maxHP);
            };
        }
    }
    public GameObject bulletVFX;
    [ServerRpc(RequireOwnership = false)]
    public void spawnBulletServerRpc(ulong clientID, float bulletSpeed, Vector3 pos, Quaternion rot)
    {
        Debug.Log("spawn bullet rpc called");
        spawnBulletClientRpc(clientID, bulletSpeed, pos, rot);
    }
    [ClientRpc]
    public void spawnBulletClientRpc(ulong clientID, float bulletSpeed, Vector3 pos, Quaternion rot)
    {
        Debug.Log("spawn bullet rpc called");
        GameObject bullet = Instantiate(bulletVFX, pos, rot);

        var direction = bullet.transform.forward;
        Debug.Log("spawn bullet with param: pos " + pos + "  rot" + rot +
            "\n" + "real: pos " + bullet.transform.position + "  rot " + bullet.transform.rotation);
        var vfx = bullet.GetComponent<UnityEngine.VFX.VisualEffect>();
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
            vfx.SendEvent("onExplode");
            //vfx.SetBool("isFollowTf", false);
            Destroy(selfGO);
        });
        Destroy(bullet, 20);
    }

    //[ServerRpc]
    //public void syncA
}
