using UnityEngine;

public class alwayFaceCamera : MonoBehaviour
{
    Transform CameraTF;
    private void Start()
    {
        CameraTF = Camera.main.transform;
    }
    void Update()
    {
        transform.LookAt(CameraTF);
    }
}
