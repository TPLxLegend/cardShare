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

public class playerInArea : targetFilter
{
    playerInArea() { }
    public static playerInArea ins = new playerInArea();
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

