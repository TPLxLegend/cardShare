using Unity.Netcode;
using UnityEngine;

public class serverFunction : SingletonNetworkPersistent<serverFunction>
{
    public override void OnNetworkSpawn()
    {
       
    }
    public GameObject playerPrefab { private get; set; }
    [ServerRpc(RequireOwnership = false)]
    public void spawnPlayerServerRpc(Vector3 pos, Quaternion rot, ulong clientID)
    {
        GameObject newObject = Instantiate(playerPrefab, pos, rot);

        if (newObject.TryGetComponent(out NetworkObject no))
        {
            Debug.Log("Spawn player:"+newObject);
            no.Spawn();
        }
        ClientRpcParams clientRpcParams = new ClientRpcParams()
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };
        NetworkObjectReference netObjRef = new NetworkObjectReference(no);
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


    }


}