using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieBehaviour : MonoBehaviour
{
    public Transform player;
    public float followDistance = 20f;
    public float attackDistance = 2f;
    private NavMeshAgent _agent;
    private Animation _animation;
    private bool _isAttacking;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animation = GetComponent<Animation>();
    }

    void Update()
    {
        var distance = Vector3.Distance(transform.position, player.position);
        if (distance <= followDistance)
        {
            _agent.SetDestination(player.position);
        }

        if (distance <= attackDistance && !_isAttacking)
        {
            _agent.isStopped = true;
            StartCoroutine(AttackCoroutine());
        }
        else if (!_isAttacking)
        {
            _agent.isStopped = false;
            _animation.Play("Walk");
        }
    }

    IEnumerator AttackCoroutine()
    {
        _isAttacking = true;
        _animation.Play("Attack1");
        yield return new WaitForSeconds(_animation["Attack1"].length);
        _isAttacking = false;
    }
}