using Unity.Netcode;
using UnityEngine;

public class serverFunction : SingletonNetworkPersistent<serverFunction>
{
    public override void OnNetworkSpawn()
    {

    }
    [SerializeField] GameObject CameraPre;
    [SerializeField] GameObject playerPrefab;

    NetworkObject spawnedObj;

    [ServerRpc(RequireOwnership = false)]
    public void spawnPlayerServerRpc(Vector3 pos, Quaternion rot, ulong clientID)
    {
        Debug.Log("pos when call rpc:   " + pos);
        var newSpawned = Instantiate(playerPrefab, pos, rot);
        spawnedObj = newSpawned.GetComponent<NetworkObject>();
        spawnedObj.Spawn();

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


    [ClientRpc]//(Delivery = RpcDelivery.Unreliable)
    public void setPlayerClientRpc(NetworkObjectReference networkObject, ulong clientId = 0, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("server call to set player" + clientRpcParams.Receive.ToString());
        //clear when 
        if (NetworkManager.Singleton.LocalClientId != clientId)
        {
            Debug.Log("server send a boardcard when use this clientRPC");
            return;
        }

        if (networkObject.TryGet(out NetworkObject networkObj))
        {
            PlayerController.Instance.player = networkObj.gameObject;
        }
        var tf = networkObj.gameObject.transform;
        Instantiate(CameraPre, tf.position, tf.rotation);

        var controllRec = PlayerController.Instance.controllReceivingSystem = networkObj.gameObject.GetComponent<ControllReceivingSystem>();
        controllRec.onCurCharacterChange.AddListener(PlayerController.Instance.loadPlayerInfo);
        PlayerController.Instance.controller = controllRec.characterController;
        PlayerController.Instance.loadPlayerInfo(controllRec.curCharacterControl);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnNetObjServerRpc(Vector3 pos, Quaternion rot, NetworkObjectReference gameobjectRef)
    {
        gameobjectRef.TryGet(out NetworkObject networkObj);
        var go = Instantiate(networkObj.gameObject, pos, rot);
        spawnedObj = go.GetComponent<NetworkObject>();
        spawnedObj.Spawn();

    }


    [ClientRpc]
    public void InstantiateNetObjClientRpc(NetworkObjectReference networkObject, ulong clientId = 0, ClientRpcParams clientRpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            Debug.Log("it call by itself:" + clientId);
            return;
        }
        networkObject.TryGet(out NetworkObject networkObj);
        var go = Instantiate(networkObj.gameObject);
        //setSomethingForGO(go);
    }
}
