using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMelee : Enemy
{
    // Use this for initialization

    public override void ResetVariables()
    {
        
    }

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
    }

    // Update is called once per frame
    void Update()
    {
        if (RoomManager.instance.PlayerInCorridor) // if player is in corridor do nothing
        {
            currentState = State.Patrol;
            return;
        }

        if (!m_playerHealth.IsDead())
        {

            if (Vector3.Distance(m_playerTransform.position, transform.position) < attackRange && RoomManager.instance.PlayerInRoom) // if player is within attackRange and in room attack the player
            {
                if (!m_isAttacking) // if we are not already attacking TODO Chnage this to use State.Attack instead of bool m_isAttacking
                {
                    FaceTarget();
                    StartCoroutine(AttackTarget());
                }
            }
            else // if player is outside attackRange
            {
                currentState = State.Chase;
            }
        }
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

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
        m_isAttacking = true; // set bool to true to indicate that we are during routine

        while (!m_playerHealth.IsDead() && Vector3.Distance(m_playerTransform.position, transform.position) < attackRange && m_playerRested) // if player is not dead and player is withinRange
        {
            m_playerHealth.TakeDamage(1); // we make it take one damage
            Debug.Log("HIT");
            yield return new WaitForSeconds(attackRate); // wait for some delay before next attack
        }

        m_isAttacking = false; // else we se bool to false so we are not attacking anymore
    }

}
