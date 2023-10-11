using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class skillObj:NetworkBehaviour
{
    public List<GameObject> objInRange;
    public UnityEvent<GameObject, GameObject> collisionEnter, triggerEnter, onDestroy, triggerExit, triggerStay;
    public UnityEvent<skillObj> onUpdate;

    public GameObject source;
    /// <summary>
    /// enable the gameobject effectHit and direction of it
    /// </summary>
    /// <param name="t"></param> <summary>
    /// 
    /// </summary>
    /// <param name="t"></param>
    public void Trigger(GameObject t)
    {
        
        var effHitObj = transform.Find("effectHit").gameObject;
        effHitObj.SetActive(true);
        try
        {
            var T = (t.transform.position - transform.position).normalized;
            effHitObj.GetComponent<UnityEngine.VFX.VisualEffect>().SetVector3("dir", T);
        }
        catch
        {

        }
    }
    void Update()
    {
        onUpdate.Invoke(this);
    }
    void OnCollisionEnter(Collision collision)
    {
        collisionEnter.Invoke(gameObject, collision.gameObject);
        Debug.Log("collison:" + collision.gameObject);
    }
    void OnTriggerEnter(Collider collider)
    {
        triggerEnter.Invoke(this.gameObject, collider.gameObject);
    }
    void OnTriggerStay(Collider collider)
    {

    }
    void OnTriggerExit(Collider collider)
    {
        triggerExit.Invoke(this.gameObject, collider.gameObject);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        Debug.Log("destroy");
        triggerEnter.RemoveAllListeners();
        triggerExit.RemoveAllListeners();
        collisionEnter.RemoveAllListeners();
        onDestroy.Invoke(this.gameObject, source);
        onDestroy.RemoveAllListeners();
        onUpdate.RemoveAllListeners();
    }
}

