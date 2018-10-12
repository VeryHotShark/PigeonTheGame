using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeavy : Enemy
{

	public int projectileAmount = 5;
	public int projectileAmountVariation = 2;
	public float waitTime;

    EnemyWeapon m_enemyWeapon;
	bool m_duringRoutine;

    void Start()
    {
        base.Init();

        currentState = State.Idle;
    }

    public override void GetComponents()
    {
        base.GetComponents();
        m_enemyWeapon = GetComponent<EnemyWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
		if (RoomManager.instance.PlayerInCorridor)
        {
            currentState = State.Idle;
            return;
        }

		 if (!m_playerHealth.IsDead())
        {
            FaceTarget();

            if(Vector3.Distance(m_playerTransform.position, transform.position) > attackRange && RoomManager.instance.PlayerInRoom)
            {
                m_agent.ResetPath();
				if(currentState != State.Attack && !m_duringRoutine)
                	m_agent.destination = m_playerTransform.position;

                currentState = State.Chase;
            }
            else if (Vector3.Distance(m_playerTransform.position, transform.position) <= attackRange)
            {
                if (currentState != State.Attack && !m_duringRoutine && currentState != State.Idle)
                {
                    m_agent.ResetPath();
                    StartCoroutine(ShootSeries());
                }
            }
        }
    }

	IEnumerator ShootSeries()
	{
		currentState = State.Attack;
		m_duringRoutine = true;

		int amountToShoot = Random.Range(projectileAmount - projectileAmountVariation, projectileAmount + projectileAmountVariation + 1);

		while(amountToShoot > 0)
		{
			m_enemyWeapon.ShootProjectile();
			amountToShoot--;
			yield return new WaitForSeconds(attackRate);
		}

		yield return StartCoroutine(WaitIdle());
	}

	IEnumerator WaitIdle()
	{
		m_duringRoutine = false;
		currentState = State.Idle;

		yield return new WaitForSeconds(waitTime);

		currentState = State.Chase;
	}
}
