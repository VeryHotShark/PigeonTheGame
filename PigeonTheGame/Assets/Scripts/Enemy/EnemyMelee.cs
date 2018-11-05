using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMelee : Enemy
{
    // Use this for initialization

    int m_moving = Animator.StringToHash("Moving");
    int m_shooting = Animator.StringToHash("Shooting");

    public override void Init()
    {
        base.Init();

        if (!m_playerHealth.IsDead())
        {
            currentState = State.Chase;
            StartCoroutine(UpdatePath()); // Start our routine to chase our player
        }

        if (waypoints != null)
        {
            currentState = State.Patrol;
            StartCoroutine(GoToNextWaypoint());
        }

        m_isAttacking = false;
    }

    // Update is called once per frame
    public void Update()
    {

        if (RoomManager.instance.PlayerInCorridor) // if player is in corridor do nothing
        {
            currentState = State.Patrol;
            return;
        }

         if (delayWaited == false) // if we haven't waited for some delay when player came into room
            StartCoroutine(WaitTimeCoroutine()); // we call routine to delay our enemy for some delay before being activated 

        if (!m_playerHealth.IsDead() && !m_health.IsDead() && delayWaited)
        {

            if (Vector3.Distance(m_playerTransform.position, transform.position) < attackRange && RoomManager.instance.PlayerInRoom) // if player is within attackRange and in room attack the player
            {
                if (currentState != State.Attack) // if we are not already attacking TODO Chnage this to use State.Attack instead of bool m_isAttacking
                {
                    m_anim.SetBool(m_moving, false);
                    FaceTarget();
                    StartCoroutine(AttackTarget());
                }
            }
            else // if player is outside attackRange
            {
                m_anim.SetBool(m_moving, true);
                currentState = State.Chase;
            }
        }
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.2f;

        while (!m_playerHealth.IsDead())
        {
            if (currentState == State.Chase && RoomManager.instance.PlayerInRoom) // if we are in room and our state is Chasing
            {
                Vector3 dirToTarget = (m_playerTransform.position - transform.position).normalized; // we calculate the dirToTarget
                Vector3 targetPosition = m_playerTransform.position - (dirToTarget * stopDistance); // and substract small amount from our playerPosition so our enemy won't go through our Player

                if (!m_health.IsDead()) // if we are not dead
                    m_agent.SetDestination(targetPosition); // we set our agent destination
            }

            yield return new WaitForSeconds(refreshRate); // wait for refreshRate and come back to top
        }
    }

    IEnumerator AttackTarget()
    {
        currentState = State.Attack; // change state to Attack

        while (!m_playerHealth.IsDead() && Vector3.Distance(m_playerTransform.position, transform.position) < attackRange) // if player is not dead and player is withinRange
        {
            if (m_playerRested)
            {
                m_anim.SetTrigger(m_shooting);
                FaceTarget();
            }

            yield return new WaitForSeconds(attackRate); // wait for some delay before next attack
        }

        currentState = State.Chase; // change state to Attack
    }


    public void DealDamage()
    {
        m_playerHealth.TakeDamage(1); // we make it take one damage
                                      //Debug.Log("EVENT");
    }

}
