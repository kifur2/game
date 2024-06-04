using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public Transform playerTransform;
    public Target enemyTarget;
    private const string AttackAudioClipName = "Enemy Attack";
    public float followDistance = 20f;
    public float attackDistance = 2f;
    public int attackDamage = 10;
    private const float AttackCooldown = 1.0f; // Cooldown in seconds
    private float _lastAttackTime = 0;
    private NavMeshAgent _agent;
    private Animation _animation;
    private bool _isAttacking;
    private BoxCollider[] _boxColliders;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animation = GetComponent<Animation>();
        _boxColliders = GetComponentsInChildren<BoxCollider>();
    }

    void Update()
    {
        if(enemyTarget.health > 0) { 
			var distance = Vector3.Distance(transform.position, playerTransform.position);
			if (distance <= followDistance)
			{
			    _agent.SetDestination(playerTransform.position);
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
    }

    IEnumerator AttackCoroutine()
    {
        _isAttacking = true;
        EnableAttack();
        _animation.Play("Attack1");
        
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySfxAtPosition(AttackAudioClipName, gameObject.transform.position, 1f);
        }
        
        yield return new WaitForSeconds(_animation["Attack1"].length);
        DisableAttack();
        _isAttacking = false;
    }

    private void EnableAttack()
    {
        foreach (var boxCollider in _boxColliders)
        {
            boxCollider.enabled = true;
        }
    }

    private void DisableAttack()
    {
        foreach (var boxCollider in _boxColliders)
        {
            boxCollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !(Time.time > _lastAttackTime + AttackCooldown)) return;
        
        var player = other.GetComponent<PlayerProperties>();
        player.TakeDamage(attackDamage);
        _lastAttackTime = Time.time;
    }
}