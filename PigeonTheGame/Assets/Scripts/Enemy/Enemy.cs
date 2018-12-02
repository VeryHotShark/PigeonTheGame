using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    public WaypointNetwork waypoints; // Waypoint our enemy will move on when patrolling
    public RoomIndex roomIndex;

    public event System.Func<RoomIndex,bool> OnEnemyDie;

    public enum State  // States our enemy can be in
    {
        Patrol,
        Chase,
        Attack,
        Idle,
        Moving
    }

    protected State currentState = State.Idle; // our enemy current state that he is in

    public enum MoveType // Patrol Move Type
    {
        PingPong,
        Loop,
        Static
    }


    [Header("Movement")]
    [Space]
    public MoveType moveType;
    public float moveSpeed = 5f;
    public float stopDistance = 1.5f;

    [Space]
    [Header("Attack")]
    [Space]
    public int attackPower = 1;
    public float attackRange = 1f;
    public float attackRate = 3f;

    [Space]
    [Header("Utilities")]
    [Space]
    public float waitTimeWhenEnter = 0f;
    public float waitTimeOnWaypoint = 1f;
    public float waitTimeWhenPlayerGetShot = 1f;

    protected bool delayWaited = false;
    protected bool delayRoutine;
    protected bool m_isAttacking;
    protected bool m_playerRested = true;
    protected bool m_reset = false;
    protected EnemyHealth m_health;
    protected PlayerHealth m_playerHealth;
    protected Transform m_playerTransform;
    protected NavMeshAgent m_agent;
    protected Animator m_anim;
    protected Vector3 m_currentWaypoint;
    protected Vector3 m_targetWaypoint;
    protected SpawnPoint m_spawnPoint;
    protected AudioSource m_audioSource;

    public SpawnPoint spawnPoint { get { return m_spawnPoint; } set { m_spawnPoint = value; } }

    public EnemyHealth enemyHealth { get { return m_health; } set { m_health = value; } }

    public bool Reset { get { return m_reset; } set { m_reset = value; } }

    int m_currentWaypointIndex = 0;

    // Use this for initialization
    public virtual void Init()
    {
        EnemyManager.instance.Enemies.Add(this); // On Start we add this enemy to our EnemyManager.Enemies list
        EnemyManager.instance.EnemyCount++; // and we increment the count of our enemy

        m_health.OnEnemyDeath += EnemyManager.instance.DecreaseEnemyCount; // and we make our EnemyManager to subscribe to our deathEvent so after death EnemyManager will automatically decrease enemies Count
        m_health.OnEnemyDeath += UnsubscribeFromPlayer;
        m_health.OnEnemyDeath += EnemyDied;

        m_playerHealth.OnPlayerLoseHealth += yieldForGivenTime;
        //PlayerHealth.OnPlayerRespawn += ResetVariables;

        m_health.Init();

        if (waypoints != null)
        {
            waypoints.waypointsArray[m_currentWaypointIndex].position = transform.position;
            m_currentWaypoint = waypoints.waypointsArray[0].position;
        }

        currentState = State.Idle;

        if(m_agent != null)
            SetNavMeshAgent();
    }

    public virtual void GetComponents()
    {

        m_agent = GetComponent<NavMeshAgent>();
        m_health = GetComponent<EnemyHealth>();
        m_audioSource = GetComponent<AudioSource>();
        m_anim = GetComponentInChildren<Animator>();
        m_playerHealth = FindObjectOfType<PlayerHealth>();
        m_playerTransform = m_playerHealth.gameObject.transform;
    }

    public virtual void SetNavMeshAgent()
    {
        if(m_agent)
        {
            m_agent.isStopped = true;
            m_agent.ResetPath();

            m_agent.isStopped = false;

            int randomiseSpeed = UnityEngine.Random.Range(-3,4);

            m_agent.speed = moveSpeed + randomiseSpeed;
            m_agent.acceleration = moveSpeed + 10f + randomiseSpeed;
        }
    }


    public virtual void FaceTarget()
    {
        Quaternion lookRotation = Quaternion.LookRotation((m_playerTransform.position - transform.position).normalized); // rotate our enemy to look at player
        transform.rotation = lookRotation;
    }

    public virtual IEnumerator GoToNextWaypoint()
    {
        GetNextWaypoint();
        yield return new WaitForSeconds(waitTimeOnWaypoint);
        StartCoroutine(GoToWaypoint());
    }

    public virtual void GetNextWaypoint()
    {
        m_currentWaypoint = transform.position; // set our current waypoint to be our position

        // depending on moveType we calculate what our next waypoint should be	
        switch (moveType)
        {
            case MoveType.PingPong: // if it is PingPong
                {
                    if (m_currentWaypointIndex < waypoints.waypointsArray.Length - 1) // if our current waypoint index is less than the length of our waypoints array
                    {
                        m_currentWaypointIndex++; // we increment our waypointIndex

                        if (m_currentWaypointIndex == waypoints.waypointsArray.Length - 1) // if we are at the end of our path (waypointNetwork)
                        {
                            Array.Reverse(waypoints.waypointsArray); // we reverse our waypointArray
                            m_currentWaypointIndex = 0; // and reset our waypoint Index
                        }
                    }
                }
                break;

            case MoveType.Loop: // if it is Loop
                {
                    m_currentWaypointIndex = (m_currentWaypointIndex + 1) % waypoints.waypointsArray.Length; // we set our waypoint Index to be the modulo ( rest from the division) of our waypointArray length. So when for example index is 3 and array length is 3 the rest will be 0 so basically we go back to beginning
                }
                break;
        }

        m_targetWaypoint = waypoints.waypointsArray[m_currentWaypointIndex].position; // Set our targetWaypoint to be nextWaypoint in our waypointsArray based on our waypointIndex we calculate before

    }

    public virtual IEnumerator GoToWaypoint()
    {
        m_agent.SetDestination(m_targetWaypoint);

        while (Vector3.Distance(transform.position, m_targetWaypoint) > 2f && RoomManager.instance.PlayerInCorridor)
        {

            if (RoomManager.instance.PlayerInRoom)
                yield break;

            yield return null;
        }

        StartCoroutine(GoToNextWaypoint());
    }

    public virtual IEnumerator WaitTimeCoroutine() // wait for some delay before attacking player
    {
        delayRoutine = true;
        yield return new WaitForSeconds(waitTimeWhenEnter);
        delayWaited = true;
    }

    public virtual void yieldForGivenTime(int playerHealth)
    {
        if (playerHealth > 0 && !m_health.IsDead() && gameObject.activeSelf)
            StartCoroutine(YieldForPlayer());
    }

    public virtual IEnumerator YieldForPlayer() // wait for some delay before attacking player
    {
        m_playerRested = false;
        yield return new WaitForSeconds(waitTimeWhenPlayerGetShot);
        m_playerRested = true;
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public virtual void UnsubscribeFromPlayer(EnemyHealth enemy)
    {
        //PlayerHealth.OnPlayerRespawn -= ResetVariables;
        m_playerHealth.OnPlayerLoseHealth -= yieldForGivenTime;


        enemy.OnEnemyDeath -= EnemyDied;
        enemy.OnEnemyDeath -= UnsubscribeFromPlayer;
    }

    public virtual void EnemyDied(EnemyHealth enemy)
    {
        if (spawnPoint != null)
        {

            if(OnEnemyDie!= null)
                OnEnemyDie(roomIndex);

            spawnPoint.EnemyAlive = false;
            spawnPoint.MyEnemy = null;
            StopAllCoroutines();
            
            if(m_agent!= null)
                m_agent.ResetPath();
        }
    }

    public virtual void ResetVariables()
    {
        transform.position = m_spawnPoint.transform.position;
        transform.rotation = m_spawnPoint.transform.rotation;

        if(m_agent != null)
        {
            m_agent.isStopped = true;
            m_agent.ResetPath();
            m_agent.isStopped = false;
            m_agent.Warp(m_spawnPoint.transform.transform.position);
        }


        delayWaited = false;
        delayRoutine = false;


        waitTimeWhenEnter = m_spawnPoint.waitDelay;
        roomIndex = m_spawnPoint.roomIndex;


        Init();
    }

    public virtual void ResetAdditionalSpawnValues()
    {
        if (spawnPoint != null)
        {
            UnsubscribeFromPlayer(m_health);
            
            spawnPoint.EnemyAlive = false;
            spawnPoint.MyEnemy = null;
            StopAllCoroutines();

            if(m_agent!= null)
                m_agent.ResetPath();

            gameObject.SetActive(false);
            
        }
    }

}
