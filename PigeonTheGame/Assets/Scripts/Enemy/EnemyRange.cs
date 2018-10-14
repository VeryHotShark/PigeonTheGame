using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyWeapon))]
public class EnemyRange : Enemy
{

    //public WaypointNetwork patrolPath;

    [Header("Movement")]
    public float walkRadius;
    public float viewAngle;

    EnemyWeapon m_enemyWeapon;

    int m_moving = Animator.StringToHash("Moving");
    int m_shooting = Animator.StringToHash("Shooting");


    // Use this for initialization
    void Start()
    {
        base.Init();

        currentState = State.Idle; // our default state is Idle
    }

    public override void GetComponents()
    {
        base.GetComponents();
        m_enemyWeapon = GetComponent<EnemyWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (RoomManager.instance.PlayerInCorridor) // if player is in corridor do nothing
        {
            currentState = State.Idle;
            return;
        }

        if (delayWaited == false) // if we haven't waited for some delay when player came into room
            StartCoroutine(WaitTimeCoroutine()); // we call routine to delay our enemy for some delay before being activated 

        if (!m_playerHealth.IsDead() && delayWaited) // if player is not dead and we waited some delay
        {
            FaceTarget();

            if (Vector3.Distance(m_playerTransform.position, transform.position) > attackRange && RoomManager.instance.PlayerInRoom) // if player is outside attackRange 
            {
                m_anim.SetBool(m_moving, true);

                currentState = State.Chase; // we set state to chase

                m_agent.ResetPath();
                //StopAllCoroutines();
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


    IEnumerator AttackTarget()
    {
        //FaceTarget();

        currentState = State.Attack; // set state to Attack

        yield return new WaitForSeconds(0.3f); // wait for very small delay when reaching new Pos before shooting a shot 

        m_anim.SetTrigger(m_shooting);
        m_enemyWeapon.ShootProjectile(); // spawn projectile

    }

    IEnumerator MoveToRandomPos()
    {
        //currentState = State.Moving;

        if (m_playerRested)
            yield return StartCoroutine(AttackTarget()); // Start our shoot routine and wait till it finish

        m_anim.SetBool(m_moving, true);
        currentState = State.Moving; // change our state to moving

        NavMeshHit hit; // store the result of our Check if there is nav mesh in specified place

        Vector3 randomPoint = transform.position + Random.insideUnitSphere * walkRadius; // calculate the random point withing our walk radius
        randomPoint.y = transform.position.y; // set the y value of that random point to be the same as enemy y value
        Vector3 randomPosOnNavMesh = Vector3.zero; // declaring a Vector3 to store randomPosOnNavMesh

        if (NavMesh.SamplePosition(randomPoint, out hit, 1f, NavMesh.AllAreas)) // if there is navMesh on random point that we specified
        {
            randomPosOnNavMesh = hit.position; // we assign that navMesh position to our randomPosOnNavMesh variable
            m_agent.SetDestination(randomPosOnNavMesh); // and we set our agent to move there
        }
        else // if there is no navMesh in specified position we sample random position till we get the random pos that is on our navMesh || TODO to fix because now randomPoint is generated only one time so we will never get to sample other randomPoint so basically we can get stuck and stay in one position
        {
            while (NavMesh.SamplePosition(randomPoint, out hit, 1f, NavMesh.AllAreas) == false) // while the result of sampling position is false we keep doing that till we get the good random point
            {
                NavMesh.SamplePosition(randomPoint, out hit, 1f, NavMesh.AllAreas);
                randomPosOnNavMesh = hit.position;
                m_agent.SetDestination(randomPosOnNavMesh);

                yield return null;
            }
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
