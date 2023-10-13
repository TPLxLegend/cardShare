using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
public enum skillMoveType
{
    line, ziczac, trace, circle,tele,notMove
}
public partial class Dic
{
    public Dictionary<skillMoveType, SkillMoveType> moveTypes = new Dictionary<skillMoveType, SkillMoveType>(){
        {skillMoveType.line,line.ins},
        {skillMoveType.ziczac,ziczac.ins} ,
        {skillMoveType.trace,trace.ins},
        {skillMoveType.circle,circle.ins},
        {skillMoveType.tele,tele.ins},
        {skillMoveType.notMove,notMove.ins},
    };
}

public interface SkillMoveType
{
    public static skillMoveType ins;
    public static int a;
    public void move(GameObject seft, Vector3 targetPosition, float speed)
    {

    }
    public async void addMoveAsync(GameObject seft, Vector3 targetPosition, float speed, int delayTime)
    {
        await Task.Delay(delayTime);
        seft.GetComponent<skillObj>().onUpdate.AddListener((s) =>
        {
            move(seft, targetPosition, speed);
        });
    }
}
public class notMove : SkillMoveType
{
    notMove(){}
    public static notMove ins=new notMove();
}
public class line : SkillMoveType
{
    line() { }
    public static line ins = new line();

    public void move(GameObject seft, Vector3 targetPosition, float speed)
    {
        seft.transform.position += (targetPosition - seft.transform.position).normalized * speed * Time.deltaTime;
    }
}
public class ziczac : SkillMoveType
{
    ziczac() { }
    public static ziczac ins = new ziczac();

    public void move(GameObject seft, Vector3 targetPosition, float speed)
    {

    }
}
public class trace : SkillMoveType
{
    trace() { }
    public static trace ins = new trace();

    public void move(GameObject seft, Vector3 targetPosition, float speed)
    {
        if (seft.TryGetComponent(out NavMeshAgent agent))
        {
            agent.SetDestination(targetPosition);
        }
    }
}

public class circle : SkillMoveType
{
    circle() { }
    public static circle ins = new circle();

    public void move(GameObject seft, Vector3 targetPosition, float speed)
    {
        seft.transform.RotateAround(targetPosition, Vector3.up, speed * Time.deltaTime);
    }
}

public class tele:SkillMoveType{
    tele(){}
    public static tele ins=new tele();
    public void move(GameObject seft, Vector3 targetPosition, float speed)
    {
        seft.transform.position = targetPosition;
    }

}