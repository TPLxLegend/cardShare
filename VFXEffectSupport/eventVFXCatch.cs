using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class VFXOutputEventCustomScript : VFXOutputEventAbstractHandler
{
    public override bool canExecuteInEditor => throw new System.NotImplementedException();

    public void CallCustomScript(VFXEventAttribute eventAttribute)
    {
        //var hit=eventAttribute.get
    }

    public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
    {
        Debug.Log("event catch : " + eventAttribute);
        if (eventAttribute.GetBool("explode"))
        {
            CallCustomScript(eventAttribute);
        }
    }
}
