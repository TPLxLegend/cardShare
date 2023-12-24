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
    public NetworkVariable<int> hp;
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
    public UnityEvent<characterInfo, Effect> onChain;
    public UnityEvent<characterInfo> onSpawn, onDie, onAttacked;
    public virtual void takeDamage(int dmg, DmgType dmgType)
    {
        NativeArray<int> Hp = new NativeArray<int>(1, Allocator.TempJob);
        Hp[0] = hp.Value;
        DamageCalcJob dmgCalc = new DamageCalcJob()
        {
            HP = Hp,
            Dmg = dmg,
            defense = defence[dmgType],
            scaleDefense = 100,
            critialRate = critRate,
            critialScaleAddition = critDmg
        };
        JobHandle handle = dmgCalc.Schedule();
        onAttacked.Invoke(this);
        Debug.Log("testing player:" + this + " take dame:" + dmg);
        handle.Complete();
        hp.Value = dmgCalc.HP[0];
        Hp.Dispose();
        Debug.Log(hp);
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
    public override void OnNetworkSpawn()
    {
        hp = new NetworkVariable<int>(maxHP, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    }

    protected virtual void OnEnable()
    {
        //xu li loading dau game
        onSpawn.Invoke(this);
        hp.OnValueChanged += checkDie;

    }
    protected virtual void OnDisable()
    {
        hp.OnValueChanged -= checkDie;
    }

    private void checkDie(int previousValue, int newValue)
    {
        if (hp.Value <= 0)
        {
            Debug.Log(name + "   ----------------Die");
            hp.Value = 0;
            onDie.Invoke(this);
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
    public byte critialRate;
    public int critialScaleAddition;
    public void Execute()
    {
        Unity.Mathematics.Random rd = new Unity.Mathematics.Random(10);
        float t = rd.NextFloat();
        Debug.Log("crit rate:" + critialRate + "  t:" + t);
        bool isCrit = t < (critialRate / 100f);
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

