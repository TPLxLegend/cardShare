using System.Collections;
using UnityEngine;
[RequireComponent(typeof(MeshRenderer))]
public class dissolve : MonoBehaviour
{
    [SerializeField] int duration;
    [SerializeField] float timeStep;
    Material material;
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }
    void Update()
    {

    }
    public void RunDisolve()
    {
        StartCoroutine(Disolve());
    }
    public IEnumerator Disolve()
    {
        for (float i = 0; i < duration; i += timeStep)
        {
            material.SetFloat("_dissolveValue",i);
            yield return new WaitForSeconds(timeStep);
        }
    }
}
