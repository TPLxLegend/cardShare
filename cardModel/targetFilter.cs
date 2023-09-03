using System.Collections.Generic;
using UnityEngine;

public static class targetDetect
{
    public static class playerInArea
    {
        public static void OnChain(GameObject gameObject,List<GameObject> targets)
        {
            playerInfo player;
            if(gameObject.TryGetComponent<playerInfo>(out player)){
                targets.Add(gameObject);
            }
        }
        public static void OffChain(GameObject gameObject,List<GameObject> targets)
        {
            targets.Remove(gameObject);
        }
    }
}

