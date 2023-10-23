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
            Debug.Log("onChain:" + go);
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
        playerInfo player;
        if (go.TryGetComponent(out player))
        {
            Debug.Log("detected:" + player);
            return true;
        }
        return false;
    }
}
public class enemyFilter:targetFilter{
    enemyFilter(){}
    public static enemyFilter ins=new enemyFilter();
    public bool Filter(GameObject go)
    {
       
        if (go.TryGetComponent(out characterInfo player))
        {
            Debug.Log("detected:" + player);
            return (player.teamID!= PlayerController.Instance.playerInfo.teamID);
        }
        return false;
    }
}
public class allyFilter:targetFilter{
    allyFilter(){}
    public static allyFilter ins=new allyFilter();
    public bool Filter(GameObject go)
    {
        if (go.TryGetComponent(out characterInfo player))
        {
            if(player==PlayerController.Instance.playerInfo) return false;
            Debug.Log("detected:" + player);
            return player.teamID==PlayerController.Instance.playerInfo.teamID;  
        }
        return false;
    }
}
       
public class seftFilter:targetFilter{
    seftFilter(){}
    public static seftFilter ins=new seftFilter();
    public bool Filter(GameObject go)
    {
        if (go.TryGetComponent(out characterInfo player))
        {
            if(player==PlayerController.Instance.playerInfo) return true;
       
        }
        return false;
    }
}
       


