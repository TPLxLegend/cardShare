using System;
using UnityEngine;
using UnityEngine.VFX;

public interface TriggerType
{
    public static TriggerType ins;
    public void addTrigger(skillObj skillObj, Effect[] effects);

}
public class hitTrigger : TriggerType // dung bat ky thu gi ngoai tru nen 
{
    public virtual void addTrigger(skillObj skillObj, Effect[] effects)
    {
        skillObj.collisionEnter.AddListener((g1, g2) =>
        {
            Debug.Log("Resolve");
            Resolve( g2, effects, skillObj);
        });
    }
    public void Resolve( GameObject collision, Effect[] cardEffect, skillObj skillObj) // kich hoat gan eff len player or enemy 
    {
        var players = skillObj.objInRange;
        var source = skillObj.source;
        Debug.Log(Time.time);
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
    }
}
public delegate void triggerFunc(GameObject obj, GameObject collision);

public class whenHitSomething : hitTrigger
{
    whenHitSomething() { }
    public static whenHitSomething ins = new whenHitSomething();


    public override void addTrigger(skillObj skillObj, Effect[] effects)
    {
        skillObj.collisionEnter.AddListener((s, t) =>
        {
            Debug.Log("S:" + s);
            s.GetComponentsInChildren<VisualEffect>()[0].enabled = true;
        });
        base.addTrigger(skillObj, effects);
    }


}
public class WhenHitPlayer : hitTrigger
{
    WhenHitPlayer() { }
    public static WhenHitPlayer ins = new WhenHitPlayer();
    public override void addTrigger(skillObj skillObj, Effect[] effects)
    {
        skillObj.collisionEnter.AddListener((go, collision) =>
        {
            if (go.TryGetComponent(out playerInfo info))
            {
                Resolve(collision, effects, skillObj);
            }
        });
    }
}
public class Imediately : TriggerType
{
    Imediately() { }
    public static Imediately ins = new Imediately();

    public void addTrigger(skillObj skillObj, Effect[] effects)
    {
        skillObj.triggerEnter.AddListener((go,collision)=>{
            func(skillObj,collision,effects);
        });
        skillObj.triggerEnter.AddListener((s, t) =>
        {
            s.GetComponentsInChildren<VisualEffect>()[0].enabled = true;
        });
    }
    public void func(skillObj skillO,GameObject collision,Effect[] effects)
    {
        if(skillO.objInRange.Contains(collision)){
            foreach(Effect ef in effects){
                var effClone = ScriptableObject.Instantiate(ef);
                Debug.Log("effclone rate:" + effClone.effect_rate);
                Debug.Log(skillO.source);
                effClone.source = skillO.source;
                collision.GetComponent<playerInfo>().addChain(effClone);
            }
        }
    }
}