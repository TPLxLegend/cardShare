using System.Collections;
using UnityEngine;
[RequireComponent(typeof(MeshRenderer))]
public class dissolve : MonoBehaviour
{
    [SerializeField] int duration;
    [SerializeField] float timeStep;
    [SerializeField] Material dissolveMaterial;
    Material originMat;
    public void RunDisolve()
    {
        StartCoroutine(Disolve());
    }
    IEnumerator Disolve()
    {
        var mesh = GetComponent<MeshRenderer>();
        originMat = mesh.material;
        mesh.material = dissolveMaterial;
        Debug.Log("disolve mat:" + mesh.material);

        for (float i = 0; i < duration; i += timeStep)
        {
            Debug.Log("dissolve step=" + i);
            dissolveMaterial.SetFloat("_dissolveValue", i);
            yield return new WaitForSeconds(timeStep);
        }
        mesh.enabled = false;
        mesh.material = originMat;
        dissolveMaterial.SetFloat("_dissolveValue", 1);

    }
}
