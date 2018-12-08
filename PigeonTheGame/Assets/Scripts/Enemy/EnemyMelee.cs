using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMelee : Enemy
{
    // Use this for initialization

    int m_moving = Animator.StringToHash("Moving");
    int m_shooting = Animator.StringToHash("Shooting");

    IEnumerator UpdatePathRoutine;
    IEnumerator StepRoutine;

    bool m_stepSound;
    bool m_pathRoutine;

    public override void Init()
    {
        base.Init();

        currentState = State.Chase;

        
        if(m_pathRoutine == false)
        {
            UpdatePathRoutine = UpdatePath();
            StartCoroutine(UpdatePathRoutine); // Start our routine to chase our player
        }
        

        m_isAttacking = false;

        if (m_audioSource != null && m_stepSound == false)
        {
            StepRoutine = StepSoundRoutine();
            StartCoroutine(StepRoutine);
        }
        

        m_agent.Warp(m_spawnPoint.transform.transform.position);
        m_agent.ResetPath();
        m_agent.updatePosition = true;
        m_agent.isStopped = false;
    }

    IEnumerator StepSoundRoutine()
    {
        m_stepSound = true;

        while (true)
        {
            if (RoomManager.instance.PlayerCurrentRoom == roomIndex && currentState != State.Attack && !m_health.IsDead())
                m_audioSource.Play();
         
            yield return new WaitForSeconds(0.2f);
        }

        m_stepSound = false;
    }

    // Update is called once per frame
    public void Update()
    {

        if (RoomManager.instance.PlayerInCorridor || RoomManager.instance.PlayerCurrentRoom != roomIndex) // if player is in corridor do nothing
        {
            currentState = State.Patrol;
            return;
        }

        if (delayWaited == false && delayRoutine == false) // if we haven't waited for some delay when player came into room
            StartCoroutine(WaitTimeCoroutine()); // we call routine to delay our enemy for some delay before being activated 

        if (!m_playerHealth.IsDead() && !m_health.IsDead() && delayWaited)
        {

            if (Vector3.Distance(m_playerTransform.position, transform.position) < attackRange && RoomManager.instance.PlayerCurrentRoom == roomIndex) // if player is within attackRange and in room attack the player
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
        m_pathRoutine = true;

        while (!m_health.IsDead())
        {
            if (currentState == State.Chase && RoomManager.instance.PlayerCurrentRoom == roomIndex && delayWaited ) // if we are in room and our state is Chasing
            {
                Vector3 dirToTarget = (m_playerTransform.position - transform.position).normalized; // we calculate the dirToTarget
                Vector3 targetPosition = m_playerTransform.position - (dirToTarget * stopDistance); // and substract small amount from our playerPosition so our enemy won't go through our Player

                m_agent.SetDestination(targetPosition); // we set our agent destination
            }

            yield return new WaitForSeconds(refreshRate); // wait for refreshRate and come back to top
        }

        m_pathRoutine = false;

    }

    IEnumerator AttackTarget()
    {
        currentState = State.Attack; // change state to Attack

        if (m_playerRested)
        {
            m_anim.SetTrigger(m_shooting);
            FaceTarget();
        }

        yield return new WaitForSeconds(attackRate); // wait for some delay before next attack

        currentState = State.Chase; // change state to Attack
    }


    public void DealDamage()
    {
        if (Vector3.Distance(m_playerTransform.position, transform.position) < attackRange)
            m_playerHealth.TakeDamage(1); // we make it take one damage
                                          //Debug.Log("EVENT");
        AudioManager.instance.PlayClipAt("EnemyMeleeAttack", transform.position);
    }

}
