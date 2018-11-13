using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeavy : Enemy
{

    public override void ResetVariables()
    {
        base.ResetVariables();

        m_duringRoutine = false;

        m_anim.SetBool(m_moving, false);
    }

    [Header("Attack")]

    public int projectileAmount = 5;
    public int projectileAmountVariation = 2;
    public float waitTimeBeforeNextShootSeries;

    EnemyWeapon m_enemyWeapon;
    bool m_duringRoutine;

    int m_moving = Animator.StringToHash("Moving");
    int m_shooting = Animator.StringToHash("Shooting");

    public override void Init()
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
    public void Update()
    {

        if (RoomManager.instance.PlayerInCorridor) // if player is in corridor just do nothing
        {
            currentState = State.Idle;
            return;
        }

        if (!m_playerHealth.IsDead()&& !m_health.IsDead())
        {
            FaceTarget();

            if (Vector3.Distance(m_playerTransform.position, transform.position) > attackRange && RoomManager.instance.PlayerInRoom && RoomManager.instance.PlayerCurrentRoom == roomIndex) // if player is outside our attackRange and is in Room
            {
                m_agent.ResetPath(); // we reset agent path
                if (currentState != State.Attack && !m_duringRoutine) // if we are not in attack State and not during routine
                {
                    m_anim.SetBool(m_moving, true);
                    m_agent.destination = m_playerTransform.position; // we set our destination to be our player position
                }

                currentState = State.Chase; // we change State to Chase
            }
            else if (Vector3.Distance(m_playerTransform.position, transform.position) <= attackRange) // else if we are within attackRange
            {
                if (currentState != State.Attack && !m_duringRoutine && currentState != State.Idle) // if we are not already attacking
                {
                    m_anim.SetBool(m_moving, false);
                    m_agent.ResetPath(); // we reset agent path,  basically clear the path
                    StartCoroutine(ShootSeries()); // and we start ShootSeries Routine
                }
            }
        }
    }

    IEnumerator ShootSeries()
    {
        currentState = State.Attack;
        m_duringRoutine = true; // set duringRoutine to true

        int amountToShoot = Random.Range(projectileAmount - projectileAmountVariation, projectileAmount + projectileAmountVariation + 1); // calculate how many projectiles will be shot

        while (amountToShoot > 0 && m_playerRested) // while amount to shoot is greater than 0
        {
            m_anim.SetTrigger(m_shooting);
            m_enemyWeapon.ShootProjectile(m_playerTransform.position); // we spawn projectile
            amountToShoot--; // we decrease amountToShoot by one
            yield return new WaitForSeconds(attackRate); // and we wait for some time between shots
        }

        yield return StartCoroutine(WaitIdle()); // after shootSeries we can optionally wait for few seconds ex. for enemy to reload or something
    }

    IEnumerator WaitIdle()
    {
        m_duringRoutine = false; // set our bool to false
        currentState = State.Idle; // change our state to idle

        yield return new WaitForSeconds(waitTimeBeforeNextShootSeries); // wait for some delay

        currentState = State.Chase; // we change our state to Chase so we can Attack again if player is still within AttackRange
    }
}
