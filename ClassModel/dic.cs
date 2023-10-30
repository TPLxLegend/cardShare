using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Dic
{
    public static Dic singleton=new Dic();
    Dic(){}
    public Dictionary<DmgType, string> colorOfDame = new Dictionary<DmgType, string>(){
        {DmgType.Physic,"#ffffff"},
        {DmgType.Fire,"#ff0000"},
        {DmgType.Electric,"#ffff00"},
        {DmgType.Ice,"#0000ff"},
        {DmgType.Poison,"#00ff00"},
        {DmgType.Dark,"ff00ff"},
        {DmgType.Light,"#ffff99"}
    };
    public Dictionary<DmgType, Color> colorMap = new Dictionary<DmgType, Color>(){
      {DmgType.Fire,Color.red},
      {DmgType.Ice,Color.blue},
      {DmgType.Physic,Color.white},
      {DmgType.Poison,Color.green},
      {DmgType.Electric,Color.yellow}
   };
    public Dictionary<DmgType, string> nameOfDame = new Dictionary<DmgType, string>(){
        {DmgType.Physic,"vật lý"},
        {DmgType.Fire,"lửa"},
        {DmgType.Electric,"điện"},
        {DmgType.Ice,"băng"},
        {DmgType.Poison,"độc"},
        {DmgType.Dark,"bóng tối"},
        {DmgType.Light,"ánh sáng"}
    };
    public Dictionary<Type, string> nameOfEffect
    {
        get => new Dictionary<Type, string>(){
        {typeof(damage),"Sát thương"},
        {typeof(healing),"Hồi máu"},
        {typeof(shield),"Khiên"},
        {typeof(negate),"Vô hiệu"}
    };
    }
    public Dictionary<targetFilterType, targetFilter> filter = new Dictionary<targetFilterType, targetFilter>(){
        {targetFilterType.allObj,playerFilter.ins},
        {targetFilterType.enemy,enemyFilter.ins},
        {targetFilterType.player,playerFilter.ins},
        {targetFilterType.ally,allyFilter.ins},
        {targetFilterType.seft,seftFilter.ins}
    };
    public Dictionary<triggerType, TriggerType> trigger = new Dictionary<triggerType, TriggerType>(){
        {triggerType.whenHitSomething,hitTrigger.ins},
        {triggerType.whenHitPlayer,WhenHitPlayer.ins},
        {triggerType.whenHitEnemy,WhenHitEnemy.ins},
        {triggerType.whenHitAlly,WhenHitTeammate.ins},
        {triggerType.whenHitSeft,WhenHitSeft.ins},
        {triggerType.inArea,InArea.ins}
    };
    public Dictionary<typeTarget,targetMethod> targetMethod = new Dictionary<typeTarget, targetMethod>(){
        {typeTarget.onCenterScreen,centerOfScreen.ins},
        {typeTarget.quickSeft,quickSeft.ins}
        
    };
}
