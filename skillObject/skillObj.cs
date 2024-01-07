using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody), typeof(NetworkObject), typeof(clientNetworkTransform))]
public class skillObj : NetworkBehaviour
{
    public List<GameObject> objInRange;
    /// <summary>
    /// g1 is this, g2 is collide object
    /// </summary>
    public UnityEvent<GameObject, GameObject> collisionEnter = new UnityEvent<GameObject, GameObject>();
    public UnityEvent<GameObject, GameObject> triggerEnter = new UnityEvent<GameObject, GameObject>();
    public UnityEvent<GameObject, GameObject> onDestroy = new UnityEvent<GameObject, GameObject>();
    public UnityEvent<GameObject, GameObject> triggerExit = new UnityEvent<GameObject, GameObject>();
    public UnityEvent<GameObject, GameObject> triggerStay = new UnityEvent<GameObject, GameObject>();
    public UnityEvent<skillObj> onUpdate = new UnityEvent<skillObj>();

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

        var effHitObj = transform.Find("effectHit");
        if (effHitObj) effHitObj.gameObject.SetActive(true);
        try
        {
            var T = (t.transform.position - transform.position).normalized;
            if (effHitObj.TryGetComponent(out VisualEffect vfx))
            {
                vfx.SetVector3("dir", T);
            }

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
        //Debug.Log("collison:" + collision.gameObject + "\n collide pos: " + collision.contacts[0].point);
    }
    void OnTriggerEnter(Collider collider)
    {
        triggerEnter.Invoke(this.gameObject, collider.gameObject);
        // Debug.Log("\ttrigger enter: " + collider.gameObject + " at " + collider.ClosestPointOnBounds(transform.position));
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
        //Debug.Log("destroy");
        triggerEnter.RemoveAllListeners();
        triggerExit.RemoveAllListeners();
        collisionEnter.RemoveAllListeners();
        onDestroy.Invoke(this.gameObject, source);
        onDestroy.RemoveAllListeners();
        onUpdate.RemoveAllListeners();
    }


}

