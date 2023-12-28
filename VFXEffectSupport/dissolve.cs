using System.Collections;
using UnityEngine;
[RequireComponent(typeof(MeshRenderer))]
public class dissolve : MonoBehaviour
{
    public float duration = 2;
    [SerializeField] float timeStep = 0.1f;
    Material originMat;

    public void RunDisolve()
    {
        StartCoroutine(Disolve());
    }
    IEnumerator Disolve()
    {
        var meshes = GetComponentsInChildren<Renderer>();
        foreach (var mesh in meshes)
        {
            originMat = mesh.material;
            mesh.material = new Material(originMat);
            Debug.Log("disolve mat:" + mesh.material);

            for (float i = 0; i < 1; i += timeStep)
            {
                Debug.Log("dissolve step=" + i);
                mesh.material.SetFloat("_dissolve", i);
                yield return new WaitForSeconds(timeStep * duration);
            }
            mesh.enabled = false;
            mesh.material = originMat;
        }
    }
}
