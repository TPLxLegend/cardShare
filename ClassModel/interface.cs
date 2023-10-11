using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class characterInfo : NetworkBehaviour
{
    public int hp;
    public int maxHP;
    public byte mp;
    public byte teamID;
    public byte maxMP;
    public Dictionary<DmgType, byte> defence = new Dictionary<DmgType, byte>(){
        {DmgType.Physic,1},
        {DmgType.Fire,0},
        {DmgType.Ice,0},
        {DmgType.Electric,0},
        {DmgType.Poison,0},
        {DmgType.Light,0},
        {DmgType.Dark,0},
    };
    public int attack;
    public byte speed;
    public List<Effect> chainEffect;
    public UnityEvent<characterInfo, Effect> onChain;
    public virtual void takeDamage(int dmg, DmgType dmgType)
    {
        NativeArray<int> Hp = new NativeArray<int>(1, Allocator.TempJob);
        Hp[0] = hp;
        DamageCalcJob dmgCalc = new DamageCalcJob()
        {
            HP = Hp,
            Dmg = dmg,
            defense = defence[dmgType],
            scaleDefense = 100
        };
        JobHandle handle = dmgCalc.Schedule();
        handle.Complete();
        hp = dmgCalc.HP[0];
        Hp.Dispose();
        Debug.Log(hp);
    }
    public virtual void healing(int heal) { }
    public virtual void addChain(Effect effect)
    {
        Debug.Log("on chain listener count:" + (onChain.GetPersistentEventCount()));
        if (onChain.GetPersistentEventCount() != 0)
        {
            onChain.Invoke(this, effect);
        }
        else
        {
            StartCoroutine(effect.trigger(gameObject));
            chainEffect.Add(effect);
        }
    }
}

[BurstCompile]
public struct DamageCalcJob : IJob
{
    public NativeArray<int> HP;
    public int Dmg;
    public int defense;
    public int scaleDefense;
    public void Execute()
    {
        HP[0] -= Dmg * (scaleDefense / (scaleDefense + defense));
    }
}