using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{

	public enum State
	{
		Patrol,
		Chase,
		Attack,
		Idle,
		Moving
	}

	protected State currentState = State.Idle;

	public float moveSpeed = 5f;
	public int attackPower = 1;
	public float attackRange = 1f;
	public float attackRate = 3f;
	public float stopDistance = 1.5f;

	public EnemyHealth m_health;
	protected bool m_isAttacking;
	protected PlayerHealth m_playerHealth;
	protected Transform m_playerTransform;
	protected NavMeshAgent m_agent;
	protected Animator m_anim;

	// Use this for initialization
	public virtual void Init ()
	{
		GetComponents();
		SetNavMeshAgent();
	}

	public virtual void GetComponents()
	{
		m_agent = GetComponent<NavMeshAgent>();
		m_health = GetComponent<EnemyHealth>();
		m_anim = GetComponent<Animator>();
		m_playerHealth = FindObjectOfType<PlayerHealth>();
		m_playerTransform = m_playerHealth.gameObject.transform;
	}

	public virtual void SetNavMeshAgent()
	{
		m_agent.speed = moveSpeed;
	}

	
    public virtual void FaceTarget()
    {
        Quaternion lookRotation = Quaternion.LookRotation((m_playerTransform.position - transform.position).normalized);
        transform.rotation = lookRotation;
    }

	public virtual void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, attackRange);
	}
}
