#region character&skill
public enum DmgType
{
    Physic,
    Fire,
    Ice,
    Electric,
    Poison,
    Dark,
    Light,
}
public enum charJob
{
    all,
    warrior,
    magician,
    thief,
    assasin,
    knight,
    paladin,
    wizzard,
    swordman,
    archer
}
public enum typeTarget
{
    quickSeft,
    quickEnemy,
    onCenterScreen,

}
#endregion
#region card
public enum cardType
{
    attack,
    trap,
    spell,
}
public enum cardTag
{
    attack,
    buff,
    neft,
    curse,
    blessing,
    elemental,
    protect
}
public enum targetFilterType
{
    allObj,
    player,
    enemy,
    ally,
    seft
}
public enum triggerType
{
    whenHitSomething,
    whenHitPlayer,
    whenHitEnemy,
    whenHitAlly,
    whenHitSeft,
    inArea
}
#endregion
#region enemy
public enum enemyLevelSpecial : byte
{
    normal = 1,
    elite = 3,
    chaos = 10,
}
#endregion


