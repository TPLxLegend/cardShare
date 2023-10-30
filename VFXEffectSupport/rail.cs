using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class rail : MonoBehaviour
{
    public SplineContainer splineContainer; 
    private float[] t; 
    Transform[] children; 
    void Start()
    {
        int childCount = transform.childCount-1;
        t = new float[childCount];
        children=new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            t[i] = (float)i / childCount;
            children[i]=transform.GetChild(i);
        }
        
    }

    void Update()
    {
        
        for (int i = 0; i < children.Length; i++)
        {
            float3 position;
            position=splineContainer.EvaluatePosition(t[i]);

            children[i].position = new Vector3(position.x, position.y, position.z);


            t[i] += Time.deltaTime * 0.1f;
            if (t[i] >= 1.0f)
                t[i] -= 1.0f;
        }
    }
}
