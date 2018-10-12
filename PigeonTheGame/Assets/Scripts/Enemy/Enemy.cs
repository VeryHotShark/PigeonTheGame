using System.Collections;
using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
	public WaypointNetwork waypoints;

	public enum State
	{
		Patrol,
		Chase,
		Attack,
		Idle,
		Moving
	}

	protected State currentState = State.Idle;

	public enum MoveType
	{
		PingPong,
		Loop,
		Static
	}

	public MoveType moveType;

	public float moveSpeed = 5f;
	public int attackPower = 1;
	public float attackRange = 1f;
	public float attackRate = 3f;
	public float stopDistance = 1.5f;

	public float waitTimeWhenEnter = 0f;

	protected EnemyHealth m_health;
	protected bool m_isAttacking;
	protected PlayerHealth m_playerHealth;
	protected Transform m_playerTransform;
	protected NavMeshAgent m_agent;
	protected Animator m_anim;
	protected Vector3 m_currentWaypoint;
	protected Vector3 m_targetWaypoint;

	int m_currentWaypointIndex = 0;

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

		if(waypoints != null)
			m_currentWaypoint = waypoints.waypointsArray[0].position;
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

	public virtual void GoToNextWaypoint()
	{
		GetNextWaypoint();
		m_agent.SetDestination(m_targetWaypoint);
	}

	public virtual void GetNextWaypoint()
    {
		m_currentWaypoint = transform.position;

		switch(moveType)
		{
			case MoveType.PingPong:
			{
				if(m_currentWaypointIndex < waypoints.waypointsArray.Length - 1 )
				{
					m_currentWaypointIndex++;

					if(m_currentWaypointIndex == waypoints.waypointsArray.Length - 1)
					{
						Array.Reverse(waypoints.waypointsArray);
						m_currentWaypointIndex = 0;
					}
				}
			}
			break;

			case MoveType.Loop:
			{
				m_currentWaypointIndex = (m_currentWaypointIndex + 1) % waypoints.waypointsArray.Length;
			}
			break;
		}

		m_targetWaypoint = waypoints.waypointsArray[m_currentWaypointIndex].position;

    }

	IEnumerator WaitTimeCoroutine()
	{
		yield return new WaitForSeconds(waitTimeWhenEnter);
	}

	public virtual void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, attackRange);
	}
}
