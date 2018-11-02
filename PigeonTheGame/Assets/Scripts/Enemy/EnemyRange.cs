using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyWeapon))]
public class EnemyRange : Enemy
{

    public override void ResetVariables()
    {
        base.ResetVariables();

        delayWaited = false;

        m_anim.SetBool(m_moving, false);
    }

    //public WaypointNetwork patrolPath;

    [Header("Movement")]
    public float walkRadius;
    public float viewAngle;

    EnemyWeapon m_enemyWeapon;

    int m_moving = Animator.StringToHash("Moving");
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

    // Update is called once per frame
    public void Update()
    {

        if (RoomManager.instance.PlayerInCorridor) // if player is in corridor do nothing
        {
            currentState = State.Idle;
            return;
        }

        if (delayWaited == false) // if we haven't waited for some delay when player came into room
            StartCoroutine(WaitTimeCoroutine()); // we call routine to delay our enemy for some delay before being activated 

        if (!m_playerHealth.IsDead() && delayWaited && !m_health.IsDead()) // if player is not dead and we waited some delay
        {
            FaceTarget();

            if (Vector3.Distance(m_playerTransform.position, transform.position) > attackRange && RoomManager.instance.PlayerInRoom) // if player is outside attackRange 
            {
                m_anim.SetBool(m_moving, true);

                currentState = State.Chase; // we set state to chase

                m_agent.ResetPath();
                StopAllCoroutines();
                m_agent.destination = m_playerTransform.position; // and set destination to be our player so enemy will chase us
            }
            else if (Vector3.Distance(m_playerTransform.position, transform.position) <= attackRange) // else if we are within attackRange
            {
                if (currentState != State.Moving && currentState != State.Attack) // if we are not moving (not already during moving Routine) and we are not in attackState
                {
                    m_agent.ResetPath(); // we reset path
                    StartCoroutine(MoveToRandomPos()); // and Start MoveToRandomPos routine || TODO Change so we start sAttackTarget Routine first and then we call MoveTORandomPos from AttackTarget Routine
                }
            }
            else
            {
                currentState = State.Idle; // else we set state to idle
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
        //FaceTarget();

        currentState = State.Attack; // set state to Attack

        yield return new WaitForSeconds(0.5f); // wait for very small delay when reaching new Pos before shooting a shot 

        AudioManager.instance.PlayClipAt("EnemyShoot", transform.position);
        m_anim.SetTrigger(m_shooting);
        m_enemyWeapon.ShootProjectile(m_playerTransform.position); // spawn projectile

    }

    IEnumerator MoveToRandomPos()
    {
        //currentState = State.Moving;

        if (m_playerRested)
            yield return StartCoroutine(AttackTarget()); // Start our shoot routine and wait till it finish

        m_anim.SetBool(m_moving, true);
        currentState = State.Moving; // change our state to moving

        NavMeshHit hit; // store the result of our Check if there is nav mesh in specified place

        Vector3 randomPos = new Vector3(Random.insideUnitCircle.x,0f,Random.insideUnitCircle.y) * walkRadius;
        Vector3 randomPoint = transform.position + randomPos; // calculate the random point withing our walk radius
        
        Vector3 randomPosOnNavMesh = Vector3.zero; // declaring a Vector3 to store randomPosOnNavMesh

        bool foundPos = false;

        while (foundPos == false) // while the result of sampling position is false we keep doing that till we get the good random point
        {
            if (NavMesh.SamplePosition(randomPoint, out hit, 1f, NavMesh.AllAreas)) // if there is navMesh on random point that we specified
            {
                foundPos = true;
                randomPosOnNavMesh = hit.position; // we assign that navMesh position to our randomPosOnNavMesh variable
                m_agent.SetDestination(randomPosOnNavMesh); // and we set our agent to move there
            }

            yield return null;
        }



        //    DEBUG PURPOSES

        //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //go.transform.position = randomPosOnNavMesh;
        //go.GetComponent<BoxCollider>().enabled = false;


        while (Vector3.Distance(transform.position, randomPosOnNavMesh) > 1f) // while the distance between enemy and its target position is greater than one just keep going
        {
            yield return null;
        }

        m_anim.SetBool(m_moving, false);
        currentState = State.Idle; // at the end when we reach the point we set our state to Idle so we can attack again
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, walkRadius);
    }
}
