using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyWeapon))]
public class EnemyRange : Enemy
{


    //public WaypointNetwork patrolPath;
    public float walkRadius;
    public float viewAngle;

    EnemyWeapon m_enemyWeapon;

    // Use this for initialization
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

            if (Vector3.Distance(m_playerTransform.position, transform.position) < attackRange)
            {

                if (currentState != State.Moving && currentState != State.Attack)
                {
                    StartCoroutine(MoveToTargetPos());
                }
            }
            else
            {
                currentState = State.Idle;
            }
        }

        Debug.Log(currentState);
    }


    IEnumerator AttackTarget()
    {
        currentState = State.Attack;
        m_isAttacking = true;

        yield return new WaitForSeconds(0.1f);
        m_enemyWeapon.ShootProjectile();

    }

    IEnumerator MoveToTargetPos()
    {
        Debug.Log("HElLO");

        yield return StartCoroutine(AttackTarget());

        currentState = State.Moving;

        NavMeshHit hit;

        Vector3 randomPoint = transform.position + Random.insideUnitSphere * walkRadius;
        randomPoint.y = transform.position.y;
        Vector3 randomPosOnNavMesh = Vector3.zero;

        if (NavMesh.SamplePosition(randomPoint, out hit, 1f, NavMesh.AllAreas))
        {
            randomPosOnNavMesh = hit.position;
            m_agent.SetDestination(randomPosOnNavMesh);
        }

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = randomPosOnNavMesh;
        go.GetComponent<BoxCollider>().enabled = false;

        while (Vector3.Distance(transform.position, randomPosOnNavMesh) > 1f)
        {
            Debug.Log("Going");
            yield return null;
        }


        currentState = State.Idle;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, walkRadius);
    }
}
