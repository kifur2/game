using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieBehaviour : MonoBehaviour
{
    public Transform player;
    public float followDistance = 20f;
    public float attackDistance = 2f;
    private NavMeshAgent agent;
    private Animation _animation;
    
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        _animation = GetComponent<Animation>();
    }
    
    void Update() {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= followDistance)
        {
            agent.SetDestination(player.position);
        }

        if (distance <= attackDistance)
        {
            agent.isStopped = true;
            _animation.Play("Attack1");
        }
        else
        {
            agent.isStopped = false;
            _animation.Play("Walk");
        }
    }

}
