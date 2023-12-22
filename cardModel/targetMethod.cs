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
        //Vector3 screenPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        //Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        //Debug.Log("main camera pos: " + Camera.main.transform.position);//+ "\ncur cam pos: " + Camera.current.transform.position);

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.Log(hit.transform.name);
        }

        // Trả về vị trí hit bởi raycast
        var point = hit.point;
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
