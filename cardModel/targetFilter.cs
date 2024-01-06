using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public interface targetFilter
{
    public static targetFilter ins;
    public bool Filter(GameObject go)
    {
        return true;
    }
    public void onChain(skillObj s, GameObject go)
    {
        bool res = Filter(go);
        if (res)
        {
            s.objInRange.Add(go);
        }
    }
    public void offChain(skillObj s, GameObject go)
    {
        bool res = Filter(go);
        if (res)
        {
            s.objInRange.Remove(go);
            Debug.Log("offchain:" + go);
        }
    }
    public void addFilterion(skillObj s)
    {
        s.triggerEnter.AddListener((g1, g2) =>
        {
            onChain(s, g2);
        });
        s.triggerExit.AddListener((g1, g2) =>
        {
            offChain(s, g2);
        });
    }

}

public class playerFilter : targetFilter
{
    playerFilter() { }
    public static playerFilter ins = new playerFilter();
    public bool Filter(GameObject go)
    {

        if (go == PlayerController.Instance.player)
        {
            Debug.Log("detected:" + NetworkManager.Singleton.LocalClientId);
            return true;
        }
        return false;
    }
}
public class enemyFilter : targetFilter
{
    enemyFilter() { }
    public static enemyFilter ins = new enemyFilter();
    public bool Filter(GameObject go)
    {
        if (go.TryGetComponent(out enemyInfo enemy))
        {
            Debug.Log("detected:" + enemy);
            return enemy.teamID != PlayerController.Instance.playerInfo.teamID;
        }
        return false;
    }
}
public class allyFilter : targetFilter
{
    allyFilter() { }
    public static allyFilter ins = new allyFilter();
    public bool Filter(GameObject go)
    {
        if (go.TryGetComponent(out ControllReceivingSystem player))
        {
            if (player == PlayerController.Instance.playerInfo) return false;
            Debug.Log("detected:" + player);
            return true;

        }
        return false;
    }
}

public class seftFilter : targetFilter
{
    seftFilter() { }
    public static seftFilter ins = new seftFilter();
    public bool Filter(GameObject go)
    {
        if (go.TryGetComponent(out characterInfo player))
        {
            if (player == PlayerController.Instance.playerInfo) return true;

        }
        return false;
    }
}



