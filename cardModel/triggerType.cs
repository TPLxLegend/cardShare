using UnityEngine;
using UnityEngine.VFX;

public interface TriggerType
{
    public static TriggerType ins;
    public void addTrigger(skillObj skillObj, Effect[] effects);

}
public class hitTrigger : TriggerType // dung bat ky thu gi ke ca nen 
{
    public static hitTrigger ins=new hitTrigger();
    public virtual void addTrigger(skillObj skillObj, Effect[] effects)
    {
        skillObj.collisionEnter.AddListener((g1, g2) =>
        {
            Resolve(g2, effects, skillObj);
        });
        skillObj.collisionEnter.AddListener((s, t) =>
        {
            Debug.Log("S:" + s);
            try
            {
                var effHitObj = s.transform.Find("effectHit").gameObject;
                var T = (t.transform.position - s.transform.position).normalized;
                Debug.Log(T);
                effHitObj.GetComponent<VisualEffect>().SetVector3("dir", T);
                effHitObj.SetActive(true);
            }
            catch
            {
                Debug.Log("fail to find effectHit");
            }
        });
    }
    public virtual void Resolve(GameObject collision, Effect[] cardEffect, skillObj skillObj) // kich hoat gan eff len player or enemy 
    {
        var players = skillObj.objInRange;
        var source = skillObj.source;
        var mesh=skillObj.gameObject.GetComponent<MeshRenderer>();
        mesh.enabled = false;
        skillObj.gameObject.GetComponent<MeshCollider>().enabled=false;
        skillObj.gameObject.GetComponent<SphereCollider>().enabled=false;
        Debug.Log("player Resolve:" + players.ToString() + " \nCount:" + players.Count);
        foreach (Effect effect in cardEffect)
        {
            Debug.Log("Resolve e:" + effect);

            foreach (var player in players)
            {
                Debug.Log("player affected:" + player);
                var effClone = ScriptableObject.Instantiate(effect);
                Debug.Log("effclone rate:" + effClone.effect_rate);
                effClone.source = source;
                player.GetComponent<playerInfo>().addChain(effClone);
            }
        }
        GameObject.Destroy(skillObj.gameObject, 1);
    }
}
public delegate void triggerFunc(GameObject obj, GameObject collision);

public class WhenHitPlayer : hitTrigger
{
    new public static WhenHitPlayer ins=new WhenHitPlayer();
    WhenHitPlayer() { }
    public override void Resolve(GameObject collision, Effect[] cardEffect, skillObj skillObj)
    {
        if (collision.TryGetComponent(out ICharacterInfo info))
        {
            Debug.Log("resolve");
            base.Resolve(collision, cardEffect, skillObj);
        }
    }
}
public class WhenHitEnemy : hitTrigger
{
    new public static WhenHitEnemy ins=new WhenHitEnemy();
    WhenHitEnemy() { }
    public override void Resolve(GameObject collision, Effect[] cardEffect, skillObj skillObj)
    {

        if (!collision.TryGetComponent(out ICharacterInfo info))
        {
            return;
        }
        if (info.teamID != skillObj.source.GetComponent<ICharacterInfo>().teamID)
        {
            return;
        }
        base.Resolve(collision, cardEffect, skillObj);
    }
}
public class WhenHitTeammate : hitTrigger
{
    new public static WhenHitTeammate ins=new WhenHitTeammate();
    WhenHitTeammate() { }
    public override void Resolve(GameObject collision, Effect[] cardEffect, skillObj skillObj)
    {

        if (!collision.TryGetComponent(out ICharacterInfo info))
        {
            return;
        }
        if (info.teamID == skillObj.source.GetComponent<ICharacterInfo>().teamID)
        {
            return;
        }
        base.Resolve(collision, cardEffect, skillObj);
    }
}
public class WhenHitSeft : hitTrigger
{
    new public static WhenHitSeft ins=new WhenHitSeft();
    WhenHitSeft() { }
    public override void Resolve(GameObject collision, Effect[] cardEffect, skillObj skillObj)
    {

        if (!collision.TryGetComponent(out ICharacterInfo info))
        {
            return;
        }
        if (info == skillObj.source.GetComponent<ICharacterInfo>())
        {
            return;
        }
        base.Resolve(collision, cardEffect, skillObj);
    }
}
public class Imediately : TriggerType
{
    Imediately() { }
    public static Imediately ins = new Imediately();

    public void addTrigger(skillObj skillObj, Effect[] effects)
    {
        skillObj.triggerEnter.AddListener((go, collision) =>
        {
            func(skillObj, collision, effects);
        });
        skillObj.triggerEnter.AddListener((s, t) =>
        {
            s.GetComponentsInChildren<VisualEffect>()[0].enabled = true;
        });
    }
    public void func(skillObj skillO, GameObject collision, Effect[] effects)
    {
        if (skillO.objInRange.Contains(collision))
        {
            foreach (Effect ef in effects)
            {
                var effClone = ScriptableObject.Instantiate(ef);
                Debug.Log("effclone rate:" + effClone.effect_rate);
                Debug.Log(skillO.source);
                effClone.source = skillO.source;
                collision.GetComponent<playerInfo>().addChain(effClone);
            }
        }
    }
}