using Unity.Netcode;
using UnityEngine;

public class serverFunction : SingletonNetworkPersistent<serverFunction>
{
    public GameObject playerPrefab { private get; set; }
    [ServerRpc(RequireOwnership = false)]
    public void spawnPlayerServerRpc(Vector3 pos, Quaternion rot)
    {
        GameObject newObject = Instantiate(playerPrefab, pos, rot);

        if (newObject.TryGetComponent(out NetworkObject no))
        {
            no.Spawn();
        }

        PlayerController.Instance.player = newObject;
    }

}