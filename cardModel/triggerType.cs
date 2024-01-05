using UnityEngine;
public interface TriggerType
{
    public static TriggerType ins;
    public void addTrigger(skillObj skillObj, Effect[] effects)
    {

    }

}
public class hitTrigger : TriggerType // dung bat ky thu gi ke ca nen 
{
    public static hitTrigger ins = new hitTrigger();
    public virtual void addTrigger(skillObj skillObj, Effect[] effects)
    {
        skillObj.collisionEnter.AddListener((g1, g2) =>
        {
            Resolve(g2, effects, skillObj);
            skillObj.Trigger(g2);
        });
    }

    public virtual void Resolve(GameObject collision, Effect[] cardEffect, skillObj skillObj) // kich hoat gan eff len player or enemyFilter 
    {
        var players = skillObj.objInRange;
        var source = skillObj.source;
        Debug.Log("call resolve");
        if (skillObj.gameObject.TryGetComponent(out MeshRenderer mesh))
        {
            mesh.enabled = false;
        }
        if (skillObj.gameObject.TryGetComponent(out MeshCollider meshCollider))
        {
            meshCollider.enabled = false;
        }
        if (skillObj.gameObject.TryGetComponent(out SphereCollider sphereCollider))
        {
            sphereCollider.enabled = false;
        }
        Debug.Log("player Resolve:" + players.ToString() + " \nCount:" + players.Count);
        foreach (Effect effect in cardEffect)
        {
            Debug.Log("Resolve e:" + effect);
            try
            {
                var dmgEff = effect as damage;
                Debug.Log("dmg effect  :" + dmgEff.dmg);
            }
            catch
            {

            }

            foreach (var player in players)
            {
                Debug.Log("player affected:" + player + "\n effect:" + effect.name);
                var effClone = ScriptableObject.Instantiate(effect);
                Debug.Log("effect detail:" + effect.effect_detail + "\teffclone rate:" + effClone.effect_rate);
                effClone.source = source;
                //info.addChain(effClone);
                if (player.TryGetComponent(out enemyInfo enemyInfo))
                {
                    enemyInfo.addChain(effClone);
                }
                else if (player.TryGetComponent(out ControllReceivingSystem conRec))
                {
                    conRec.curCharacterControl.GetComponent<playerInfo>().addChain(effClone);
                }
            }
        }
        GameObject.Destroy(skillObj.gameObject, 1);
    }
}
public delegate void triggerFunc(GameObject obj, GameObject collision);

public class WhenHitPlayer : hitTrigger
{
    new public static WhenHitPlayer ins = new WhenHitPlayer();
    WhenHitPlayer() { }
    public override void Resolve(GameObject collision, Effect[] cardEffect, skillObj skillObj)
    {
        if (collision.TryGetComponent(out ControllReceivingSystem _))
        {
            base.Resolve(collision, cardEffect, skillObj);
        }
    }
}
public class WhenHitEnemy : hitTrigger
{
    new public static WhenHitEnemy ins = new WhenHitEnemy();
    WhenHitEnemy() { }
    public override void Resolve(GameObject collision, Effect[] cardEffect, skillObj skillObj)
    {

        if (!collision.TryGetComponent(out enemyInfo info))
        {
            return;
        }
        base.Resolve(collision, cardEffect, skillObj);
    }
}
public class WhenHitTeammate : hitTrigger
{
    new public static WhenHitTeammate ins = new WhenHitTeammate();
    WhenHitTeammate() { }
    public override void Resolve(GameObject collision, Effect[] cardEffect, skillObj skillObj)
    {
        if (!collision.TryGetComponent(out ControllReceivingSystem conRec))
        {
            return;
        }
        if (conRec == PlayerController.Instance.controllReceivingSystem)
        {
            Debug.Log("hit self");
            return;
        }
        base.Resolve(collision, cardEffect, skillObj);
    }
}
public class WhenHitSeft : hitTrigger
{
    new public static WhenHitSeft ins = new WhenHitSeft();
    WhenHitSeft() { }
    public override void Resolve(GameObject collision, Effect[] cardEffect, skillObj skillObj)
    {

        if (!collision.TryGetComponent(out ControllReceivingSystem conRec))
        {
            return;
        }
        if (!conRec.Equals(PlayerController.Instance.controllReceivingSystem))
        {
            return;
        }
        base.Resolve(collision, cardEffect, skillObj);
    }
}
public class InArea : TriggerType
{
    InArea() { }
    public static InArea ins = new InArea();

    public void addTrigger(skillObj skillObj, Effect[] effects)
    {
        skillObj.triggerEnter.AddListener((s, t) =>
        {
            //if(skillObj.){return;}
            Debug.Log("trigger enter in area:" + t.name);
            func(skillObj, t, effects);
            skillObj.Trigger(t);
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
                effClone.source = skillO.source;
                collision.GetComponent<ControllReceivingSystem>().curCharacterControl.gameObject.GetComponent<playerInfo>().addChain(effClone);
            }
        }
    }
}