using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Netcode;

public class CharacterInfo : NetworkBehaviour
{
    public int hp;
    public int maxHP;
    public byte mp;
    public byte teamID ;
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
    public virtual void takeDamage(int dmg, DmgType dmgType)
    {

    }
    public virtual void healing(int heal) { }
    public virtual void addChain(Effect effect) { }
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