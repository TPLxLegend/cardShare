using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class skillObj : MonoBehaviour
{
    public Transform target
    {
        set
        {
            if (!agent) agent = GetComponent<NavMeshAgent>();
            agent.SetDestination(value.position);
        }
    }
    public List<GameObject> objInRange;//co the them thu cong 
    public UnityEvent<GameObject, GameObject> collisionEnter, triggerEnter, onDestroy, triggerExit;
    Rigidbody rb;
    NavMeshAgent agent;
    public GameObject source;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
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
    }
}

