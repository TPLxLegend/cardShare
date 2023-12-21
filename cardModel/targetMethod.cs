using UnityEngine;

public interface targetMethod
{
    public static targetMethod ins;
    public Vector3 target()
    {
        return Vector3.zero;
    }
}

public class centerOfScreen : targetMethod
{
    centerOfScreen() { }
    public static centerOfScreen ins = new centerOfScreen();
    public float raycastDistance = 100f;
    public Vector3 target()
    {
        Vector3 screenPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Debug.Log("main camera pos: " + Camera.main.transform.position);//+ "\ncur cam pos: " + Camera.current.transform.position);
        // Trả về vị trí hit bởi raycast
        var point = ray.GetPoint(raycastDistance);
        Debug.Log("point:" + point);
        return point;
    }
}
public class quickSeft : targetMethod
{
    quickSeft() { }
    public static quickSeft ins = new quickSeft();
    public Vector3 target()
    {
        //thay bang character
        return PlayerController.Instance.player.transform.position;
    }
}
