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
		Idle
	}

	protected State currentState = State.Idle;

	public float moveSpeed = 5f;
	public float attackRange = 1f;
	public float attackRate = 3f;
	public float stopDistance = 1.5f;

	protected PlayerHealth m_playerHealth;
	protected Transform m_playerTransform;
	protected EnemyHealth m_health;
	protected NavMeshAgent m_agent;

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
		m_playerHealth = FindObjectOfType<PlayerHealth>();
		m_playerTransform = m_playerHealth.gameObject.transform;
	}

	public virtual void SetNavMeshAgent()
	{
		m_agent.speed = moveSpeed;
	}
}
