using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class rail : MonoBehaviour
{
    public SplineContainer spline;
    public float speed = 1f;
    private float[] t;

    void Start()
    {
        t = new float[transform.childCount - 1];

    }
    void Update()
    {
        NativeArray<float> nativeT = new NativeArray<float>(t, Allocator.TempJob);
        NativeArray<float3> positions = new NativeArray<float3>(t.Length, Allocator.TempJob);

        MoveJob job = new MoveJob
        {
            speed = this.speed,
            spline = this.spline.Spline,
            t = nativeT,
            positions = positions
        };
        JobHandle handle = job.Schedule(t.Length, 64);
        handle.Complete();

        nativeT.CopyTo(t);

        for (int i = 0; i < t.Length; i++)
        {
            transform.GetChild(i).position = positions[i];
        }

        nativeT.Dispose();
        positions.Dispose();
    }
}

struct MoveJob : IJobParallelFor
{
    public float speed;
    public Spline spline;
    public NativeArray<float> t;
    public NativeArray<float3> positions;

    public void Execute(int index)
    {
        t[index] += Time.deltaTime * speed;

        if (t[index] > 1f)
        {
            t[index] -= 1f;
        }

        positions[index] = spline.EvaluatePosition(t[index]);
    }
}