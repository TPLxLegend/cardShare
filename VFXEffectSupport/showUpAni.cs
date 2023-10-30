using System.Collections;
using UnityEngine;

public class showUpAni : MonoBehaviour
{
    bool run;
    public float stepTime;
    public float duration;
    Material mat;
    void Start()
    {
        
    }
    void OnEnable(){
        try{
            mat = GetComponent<MeshRenderer>().material;
        Run();
        }catch{
            Debug.Log("show up error: mat is null??????");
        }
        
    }
    public void Run(){
        StartCoroutine(changeValue());
    }
    public IEnumerator changeValue(){
        run = true;
        for(float t=0;t<duration;t+=stepTime){
            mat.SetFloat("_upValue", t);
            yield return new WaitForSeconds(stepTime);
        }
    }
}
