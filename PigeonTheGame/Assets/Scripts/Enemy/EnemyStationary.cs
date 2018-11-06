using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStationary : Enemy
{

    public override void ResetVariables()
    {
        m_duringRoutine = false;
    }

    [Header("Movement")]
    public int projectileAmount = 5;
    public int projectileAmountVariation = 2;
    public float waitTimeBeforeNextShootSeries;


    bool m_duringRoutine;
    EnemyWeapon m_enemyWeapon;

    int m_shooting = Animator.StringToHash("Shooting");

    public override void Init()
    {
        base.Init();
    }

    public override void GetComponents()
    {
        base.GetComponents();
        m_enemyWeapon = GetComponent<EnemyWeapon>();
    }

    public void Update()
    {

        if (RoomManager.instance.PlayerInCorridor)
        {
            currentState = State.Idle;
            return;
        }

        if (delayWaited == false)    // if we haven't waited for some delay when player came into room
            StartCoroutine(WaitTimeCoroutine()); // we call routine to delay our enemy for some delay before being activated 


        if (!m_playerHealth.IsDead() && delayWaited && !m_health.IsDead()) // if player is not dead and we waited some delay
        {
            FaceTarget();

            if (Vector3.Distance(m_playerTransform.position, transform.position) < attackRange && RoomManager.instance.PlayerInRoom) // if player is within range and player is in room
            {
                if (currentState != State.Attack && !m_duringRoutine) // if we are not in Attack state and nor during routine
                    StartCoroutine(ShootSeries()); // we start shoot series routine
            }
        }
    }

    IEnumerator ShootSeries()
    {
        currentState = State.Attack; // set state to Attack
        m_duringRoutine = true; // set duringROutine bool to true

        int amountToShoot = Random.Range(projectileAmount - projectileAmountVariation, projectileAmount + projectileAmountVariation + 1); // calculate how many projectile will be spawn

        while (amountToShoot > 0 && m_playerRested) // while projectile amount is > 0
        {
            AudioManager.instance.PlayClipAt("EnemyShoot", transform.position);
            m_anim.SetTrigger(m_shooting);
            m_enemyWeapon.ShootProjectile(m_playerTransform.position); // shoot projectile
            
            amountToShoot--; // decrement by one
            yield return new WaitForSeconds(attackRate); // wait for some delay and repeat while amount is > 0
        }

        yield return StartCoroutine(WaitIdle()); // after enemy finish shooting we can optinally wait for few seconds for example to make enemy reload
    }

    IEnumerator WaitIdle()
    {
        m_duringRoutine = false; // set during routine to false
        currentState = State.Idle; // change state to idle

        yield return new WaitForSeconds(waitTimeBeforeNextShootSeries); // wait for given time

        currentState = State.Chase; // change state to Chase si we can attack again
    }


}
