using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody),typeof(SphereCollider))]
public class skillObj : MonoBehaviour
{
    public List<GameObject> objInRange;
    public UnityEvent<GameObject, GameObject> collisionEnter, triggerEnter, onDestroy, triggerExit;
    public UnityEvent<skillObj> onUpdate;

    public GameObject source;
    void Start()
    {
        
    }
    void Update(){
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
    void OnTriggerExit(Collider collider)
    {
        triggerExit.Invoke(this.gameObject, collider.gameObject);
    }
    public void OnDestroy()
    {
        Debug.Log("destroy");
        triggerEnter.RemoveAllListeners();
        triggerExit.RemoveAllListeners();
        collisionEnter.RemoveAllListeners();
        onDestroy.Invoke(this.gameObject, source);
        onDestroy.RemoveAllListeners();
        onUpdate.RemoveAllListeners();
    }
}

