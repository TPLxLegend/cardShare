using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))]
public class skillObj : MonoBehaviour
{
    public Transform target
    {
        set
        {
            agent.SetDestination(value.position);
        }
    }
    public List<GameObject> objInRange;
    public UnityEvent<GameObject,GameObject> collisionEnter;
    public UnityEvent<GameObject,GameObject> triggerEnter;
    //Rigidbody rb;
    NavMeshAgent agent;
    void Start()
    {
        // rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    void OnCollisionEnter(Collision collision)
    {
        collisionEnter.Invoke(this.gameObject,collision.gameObject);
    }
    void OnTriggerEnter(Collider collider){
        onChain(collider.gameObject);
        triggerEnter.Invoke(this.gameObject,collider.gameObject);
    }
    void OnTriggerExit(Collider collider){
        offChain(collider.gameObject);
    }


    virtual protected void onChain(GameObject gameObject){
        targetDetect.playerInArea.OnChain(gameObject,objInRange);
    }
    virtual protected void offChain(GameObject gameObject){
        targetDetect.playerInArea.OffChain(gameObject,objInRange);
    }
}
