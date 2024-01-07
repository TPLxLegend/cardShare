using System.Collections.Generic;
using TMPro;
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
    // #region  alternative if needed 
    // public UnityEvent<ushort> onHPChange = new UnityEvent<ushort>();
    // public ushort hp
    // {
    //     get => _hp; set
    //     {
    //         onHPChange.Invoke(value);
    //         _hp = value;
    //     }
    // }
    // ushort _hp = 0;
    // [ServerRpc(RequireOwnership = false)]
    // public void setHPServerRpc(ushort value)
    // {
    //     setHPClientRpc(value);
    // }
    // [ClientRpc]
    // public void setHPClientRpc(ushort value)
    // {
    //     hp = value;
    // }
    // #endregion
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
    public byte critDmg = 50;
    public byte speed;
    public List<Effect> chainEffect;
    public UnityEvent<characterInfo, Effect> onChain = new UnityEvent<characterInfo, Effect>();
    public UnityEvent<characterInfo> onSpawn = new UnityEvent<characterInfo>();
    public UnityEvent<characterInfo> onDie = new UnityEvent<characterInfo>();
    public UnityEvent<characterInfo> onAttacked = new UnityEvent<characterInfo>();
    public virtual void takeDamage(int dmg, DmgType dmgType, byte crit = 5, byte critScale = 50)
    {
        NativeArray<int> Hp = new NativeArray<int>(1, Allocator.TempJob);
        Hp[0] = hp.Value;
        System.Random rd = new System.Random();
        int t = rd.Next(0, 100);
        var dmgCalc = new DamageCalcJob()
        {
            HP = Hp,
            Dmg = dmg,
            DmgType = dmgType,
            defense = defence[dmgType],
            scaleDefense = 100,
            critialRate = crit,
            critialScaleAddition = critScale,
            t = t,
        };
        var handle = dmgCalc.Schedule();
        onAttacked.Invoke(this);
        Debug.Log("testing :" + this + " take dame:" + dmg);
        handle.Complete();
        Debug.Log("call dmg handle:::::::::::::::::::::::::::::::::::::::::");
        var newHp = dmgCalc.HP[0];
        Debug.Log("new hp calc:" + newHp);
        afterCalcHPServerRpc(newHp, dmgType);
        Hp.Dispose();
    }

    [ClientRpc]
    public void showDmgClientRpc(int dmg, DmgType dmgType, Vector3 pos, Quaternion rot)
    {
        Debug.Log("postion param:" + pos);
        var go = Instantiate(playerGeneralInfo.Instance.dmgShowObj, pos, rot);
        go.AddComponent<alwayFaceCamera>();
        go.GetComponentInChildren<Canvas>().worldCamera = Camera.current;
        var text = go.GetComponentInChildren<TMP_Text>();
        text.text = string.Format("<color={0}> {1} </color>", Dic.singleton.colorOfDame[dmgType], dmg);
        Debug.Log("text string: " + text.text);
        Debug.Log("postion after faceCam:" + go.transform.position);

        Destroy(go, 10);
    }
    public virtual void healing(int heal) { }
    [ServerRpc(RequireOwnership = false)]
    public void afterCalcHPServerRpc(int val, DmgType dmgType)
    {
        Debug.Log("call ServerRpc set hp");
        int dmg = hp.Value - val;
        hp.Value = val;
        showDmgClientRpc(dmg, dmgType, transform.position, transform.rotation);
    }
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
    public void removeChain(Effect effect)
    {
        chainEffect.Remove(effect);
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

    public DmgType DmgType;

    public void Execute()
    {
        bool isCrit = t < critialRate;
        float dmg = Dmg;
        if (isCrit)
        {
            Debug.Log("lucky crit");
            dmg =
                Dmg *
                 (isCrit ? (critialScaleAddition * 1f / 100 + 1f) : 1f)
                ;
        }
        HP[0] -= (int)(dmg * (scaleDefense * 1f / (scaleDefense + defense)));
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

