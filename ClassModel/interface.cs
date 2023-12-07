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
    public byte speed;
    public List<Effect> chainEffect;
    public UnityEvent<characterInfo, Effect> onChain;
    public UnityEvent<characterInfo> onSpawn, onDie;
    public virtual void takeDamage(int dmg, DmgType dmgType)
    {
        NativeArray<int> Hp = new NativeArray<int>(1, Allocator.TempJob);
        Hp[0] = hp.Value;
        DamageCalcJob dmgCalc = new DamageCalcJob()
        {
            HP = Hp,
            Dmg = dmg,
            defense = defence[dmgType],
            scaleDefense = 100
        };
        JobHandle handle = dmgCalc.Schedule();
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
    public void Execute()
    {
        HP[0] -= Dmg * (scaleDefense / (scaleDefense + defense));
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

