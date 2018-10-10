using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStationary : Enemy
{

    public int projectileAmount = 5;
	public int projectileAmountVariation = 2;
    public float shootInterval;
    public float waitTime;

    bool m_duringRoutine;

    EnemyWeapon m_enemyWeapon;

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

            if (Vector3.Distance(m_playerTransform.position, transform.position) < attackRange && RoomManager.instance.PlayerInRoom)
            {
                if (currentState != State.Attack && !m_duringRoutine)
					StartCoroutine(ShootSeries());
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
			yield return new WaitForSeconds(shootInterval);
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
