using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class function
{
    //public static T[] GetAllInstances<T>() where T : ScriptableObject
    //{
    //    string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
    //    T[] instances = new T[guids.Length];
    //    for (int i = 0; i < guids.Length; i++)
    //    {
    //        string path = AssetDatabase.GUIDToAssetPath(guids[i]);
    //        instances[i] = AssetDatabase.LoadAssetAtPath<T>(path);
    //    }
    //    return instances;
    //}

    public static Vector3 float3ToVector3(float3 input)
    {
        return new Vector3(input.x, input.y, input.z);
    }
    public static Vector3[] float3ToVector3(float3[] input)
    {
        Vector3[] res = new Vector3[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            res[i] = new Vector3(input[i].x, input[i].y, input[i].z);
        }
        return res;
    }
    public static float3 vector3ToFloat3(Vector3 ip)
    {
        return new float3(ip.x, ip.y, ip.z);
    }
    public static float3[] vector3ToFloat3(Vector3[] ip)
    {
        float3[] res = new float3[ip.Length];
        for (int i = 0; i < ip.Length; i++)
        {
            res[i] = vector3ToFloat3(ip[i]);
        }
        return res;
    }

    public static void LookAtXAxis(Transform transform, Vector3 target)
    {
        Vector3 relativePos = target - transform.position;
        Quaternion rotation = Quaternion.LookRotation(Vector3.Cross(relativePos, Vector3.up), Vector3.up);
        transform.rotation = rotation;
    }


}