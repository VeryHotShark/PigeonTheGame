using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMelee : Enemy
{

    // Use this for initialization
    void Start()
    {
		base.Init();

		if(!m_playerHealth.IsDead())
		{
			currentState = State.Chase;
			StartCoroutine(UpdatePath());
		}
    }

    // Update is called once per frame
    void Update()
    {
		if(RoomManager.instance.PlayerInCorridor)
		{
			currentState = State.Idle; 
			return;
		}

		if(!m_playerHealth.IsDead())
		{

			if(!RoomManager.instance.PlayerInRoom)
			{
				currentState = State.Patrol;
				
			}

			if(Vector3.Distance(m_playerTransform.position, transform.position) < attackRange && RoomManager.instance.PlayerInRoom)
			{
				if(!m_isAttacking)
				{
					FaceTarget();
					StartCoroutine(AttackTarget());
				}
			}
			else
			{
				currentState = State.Chase;
			}
		}
    }

	IEnumerator UpdatePath()
	{
		float refreshRate = 0.25f;

		while(!m_playerHealth.IsDead())
		{
			if(currentState == State.Chase && RoomManager.instance.PlayerInRoom)
			{
				Vector3 dirToTarget = (m_playerTransform.position - transform.position).normalized;
				Vector3 targetPosition = m_playerTransform.position - (dirToTarget * stopDistance);

				if(!m_health.IsDead())
					m_agent.SetDestination(targetPosition);
			}

			yield return new WaitForSeconds(refreshRate);
		}


	}

	IEnumerator AttackTarget()
	{
		currentState = State.Attack;
		m_isAttacking = true;

		while(!m_playerHealth.IsDead() && Vector3.Distance(m_playerTransform.position, transform.position) < attackRange)
		{
			m_playerHealth.TakeDamage(1);
			Debug.Log("HIT");
			yield return new WaitForSeconds(attackRate);
		}

		m_isAttacking = false;
	}

}
