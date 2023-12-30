using Unity.Netcode;
using UnityEngine;

public class spawnPlayerSystem : SingletonNetworkPersistent<spawnPlayerSystem>
{
    [SerializeField] GameObject CameraPre;
    [SerializeField] GameObject playerPrefab;

    NetworkObject spawnedObj;
    [SerializeField] GameObject namePlayerCanvas;

    [ServerRpc(RequireOwnership = false)]
    public void spawnPlayerServerRpc(Vector3 pos, Quaternion rot, ulong clientID)
    {
        Debug.Log("pos when call rpc:   " + pos);
        Debug.Log("who call it: " + clientID);
        var newSpawned = Instantiate(playerPrefab, pos, rot);
        spawnedObj = newSpawned.GetComponent<NetworkObject>();

        spawnedObj.SpawnWithOwnership(clientID);

        //Debug.Log(spawnedObj + " is own by client: " + spawnedObj.OwnerClientId);

        ClientRpcParams clientRpcParams = new ClientRpcParams()
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };
        NetworkObjectReference netObjRef = new NetworkObjectReference(spawnedObj);
        setPlayerClientRpc(netObjRef, clientID, clientRpcParams);
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

}
