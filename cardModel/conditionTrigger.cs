using UnityEngine;

public class conditionTrigger : ScriptableObject
{
    public string describe = "";

    public virtual bool checkCondittion()
    {
        return true;
    }
}
