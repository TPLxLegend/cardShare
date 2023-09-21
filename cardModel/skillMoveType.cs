using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public enum skillMoveType{
    line,ziczac
}
public partial class Dic{
    public static Dictionary<skillMoveType,SkillMoveType> moveTypes{get =>new Dictionary<skillMoveType, SkillMoveType>(){
        {skillMoveType.line,new line()},
        {skillMoveType.ziczac,new ziczac()}    
    };}
}
public interface SkillMoveType
{
    public static skillMoveType ins;

    public void move(Rigidbody rb, Vector3 dir, float speed)
    {
        //return null;
    }
    public async void moveAsync(Rigidbody rb, Vector3 dir, float speed)
    {
        var res = Task.Run(() =>
        {
            move(rb, dir, speed);
        });
    }
}

public class line : SkillMoveType
{
    public void move(Rigidbody rb, Vector3 dir, float speed)
    {
        rb.velocity = dir.normalized * speed;
    }
}
public class ziczac : SkillMoveType
{
    public void move(Rigidbody rb, Vector3 dir, float speed)
    {

    }
}