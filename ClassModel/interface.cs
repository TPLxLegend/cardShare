using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(NetworkObject))]
public class characterInfo : NetworkBehaviour
{

    public NetworkVariable<int> hp = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public int maxHP;
    public byte mp;
    public byte teamID;
    public byte maxMP;
    public Dictionary<DmgType, byte> defence = new Dictionary<DmgType, byte>(){
        {DmgType.Physic,0},
        {DmgType.Fire,0},
        {DmgType.Ice,0},
        {DmgType.Electric,0},
        {DmgType.Poison,0},
        {DmgType.Light,0},
        {DmgType.Dark,0},
    };
    public int attack;
    /// <summary>
    /// 0 to 100 in % unit
    /// </summary>
    public byte critRate = 5;
    /// <summary>
    /// 0 to 100 in % unit, it is addition dame % when crit 
    /// </summary>
    public int critDmg = 50;
    public byte speed;
    public List<Effect> chainEffect;
    public UnityEvent<characterInfo, Effect> onChain = new UnityEvent<characterInfo, Effect>();
    public UnityEvent<characterInfo> onSpawn = new UnityEvent<characterInfo>();
    public UnityEvent<characterInfo> onDie = new UnityEvent<characterInfo>();
    public UnityEvent<characterInfo> onAttacked = new UnityEvent<characterInfo>();
    public virtual void takeDamage(int dmg, DmgType dmgType)
    {
        NativeArray<int> Hp = new NativeArray<int>(1, Allocator.TempJob);
        Hp[0] = hp.Value;
        System.Random rd = new System.Random();
        int t = rd.Next(0, 100);
        DamageCalcJob dmgCalc = new DamageCalcJob()
        {
            HP = Hp,
            Dmg = dmg,
            defense = defence[dmgType],
            scaleDefense = 100,
            critialRate = critRate,
            critialScaleAddition = critDmg,
            t = t,
        };
        JobHandle handle = dmgCalc.Schedule();
        onAttacked.Invoke(this);
        Debug.Log("testing :" + this + " take dame:" + dmg);
        handle.Complete();
        hp.Value = dmgCalc.HP[0];
        Hp.Dispose();
        Debug.Log("HP:" + hp.Value);
        Debug.Log("method:  " + hp.OnValueChanged);
    }
    public virtual void healing(int heal) { }

    public virtual void addChain(Effect effect)
    {
        Debug.Log("on chain listener count:" + onChain.GetPersistentEventCount());
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
    protected virtual void OnEnable()
    {
        //xu li loading dau game
        onSpawn.Invoke(this);
        hp.Value = maxHP;
        hp.OnValueChanged += checkDie;

    }
    protected virtual void OnDisable()
    {
        hp.OnValueChanged -= checkDie;
    }

    public void checkDie(int previousValue, int newValue)
    {
        Debug.Log("checkDie new value:" + newValue);
        if (newValue > 0) { return; }
        Debug.Log(name + "   ----------------Die");

        onDie.Invoke(this);

    }
}

[BurstCompile]
public struct DamageCalcJob : IJob
{
    public NativeArray<int> HP;
    public int Dmg;
    public int defense;
    public int scaleDefense;
    public byte critialRate;
    //t is random 0 to 99 for compare "<" with crit rate 
    public int t;
    public int critialScaleAddition;
    public void Execute()
    {
        Debug.Log("crit rate:" + (int)critialRate + "  t:" + t);
        bool isCrit = t < critialRate;
        if (isCrit)
        {
            Debug.Log("lucky crit");
        }
        HP[0] -= (int)(Dmg * (scaleDefense / (float)(scaleDefense + defense)) * (isCrit ? (critialScaleAddition / 100f + 1f) : 1f));
    }
}


public interface IData
{
    public void save()
    {

    }
    public object load()
    {
        return default;
    }
}

public interface iEnemySPBehaviour
{
    public string detail { get => ""; }
    public virtual void attackHandle()
    {

    }

}

