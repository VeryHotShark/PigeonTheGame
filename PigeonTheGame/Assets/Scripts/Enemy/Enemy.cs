using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    public WaypointNetwork waypoints; // Waypoint our enemy will move on when patrolling
    public RoomIndex roomIndex;

    //public event System.Func<RoomIndex,bool> OnEnemyDie;
    public event System.Action<RoomIndex> OnEnemyDie;

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
    public float shrinkDuration = 1f;
    public float shrinkDelay = 1f;

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
    protected Vector3 m_startSize;
    protected Vector3 m_shrinkSize;

    public SpawnPoint spawnPoint { get { return m_spawnPoint; } set { m_spawnPoint = value; } }

    public EnemyHealth enemyHealth { get { return m_health; } set { m_health = value; } }

    public bool Reset { get { return m_reset; } set { m_reset = value; } }

    int m_currentWaypointIndex = 0;

    // Use this for initialization
    public virtual void Init()
    {
        if(transform.localScale.x == 0f)
            transform.localScale = m_startSize;

        m_startSize = transform.localScale;
        m_shrinkSize = m_startSize * 0.5f;

        EnemyManager.instance.Enemies.Add(this); // On Start we add this enemy to our EnemyManager.Enemies list
        EnemyManager.instance.EnemyCount++; // and we increment the count of our enemy

        m_health.OnEnemyDeath += EnemyManager.instance.DecreaseEnemyCount; // and we make our EnemyManager to subscribe to our deathEvent so after death EnemyManager will automatically decrease enemies Count
        m_health.OnEnemyDeath += UnsubscribeFromPlayer;
        m_health.OnEnemyDeath += EnemyDied;

        m_playerHealth.OnPlayerLoseHealth += yieldForGivenTime;

        m_health.Init();

        currentState = State.Idle;

        if(m_agent != null)
            SetNavMeshAgent();
    }

    
    public virtual void UnsubscribeFromPlayer(EnemyHealth enemy)
    {
        //PlayerHealth.OnPlayerRespawn -= ResetVariables;
        m_playerHealth.OnPlayerLoseHealth -= yieldForGivenTime;


        enemy.OnEnemyDeath -= EnemyDied;
        enemy.OnEnemyDeath -= UnsubscribeFromPlayer;
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

            int randomiseSpeed = UnityEngine.Random.Range(-1,2);

            m_agent.speed = moveSpeed + randomiseSpeed;
            m_agent.acceleration = moveSpeed + 10f + randomiseSpeed;
        }
    }


    public virtual void FaceTarget()
    {
        Quaternion lookRotation = Quaternion.LookRotation((m_playerTransform.position - transform.position).normalized); // rotate our enemy to look at player
        transform.rotation = lookRotation;
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


    public virtual void EnemyDied(EnemyHealth enemy)
    {
        if (spawnPoint != null)
        {
            StartCoroutine(Shrink());

            spawnPoint.EnemyAlive = false;
            spawnPoint.MyEnemy = null;

            if(OnEnemyDie!= null)
                OnEnemyDie(roomIndex);

            OnEnemyDie -= EnemySpawner.instance.CheckIfAllDead;

            //StopAllCoroutines();
            
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

        //StopAllCoroutines();

        delayWaited = false;
        delayRoutine = false;


        waitTimeWhenEnter = m_spawnPoint.waitDelay;
        roomIndex = m_spawnPoint.roomIndex;


        Init();
    }

    public virtual void ResetAliveVariables()
    {

        transform.position = m_spawnPoint.transform.position;
        transform.rotation = m_spawnPoint.transform.rotation;

        delayWaited = false;
        delayRoutine = false;

        if(m_agent != null)
        {
            m_agent.isStopped = true;
            m_agent.ResetPath();
            m_agent.isStopped = false;
            m_agent.Warp(m_spawnPoint.transform.transform.position);
        }

        m_health.Init();
    }

    public virtual void ResetAdditionalSpawnValues()
    {
        if (spawnPoint != null)
        {
            UnsubscribeFromPlayer(m_health);
            
            spawnPoint.EnemyAlive = false;
            spawnPoint.MyEnemy = null;
            

            if(m_agent!= null)
                m_agent.ResetPath();

            gameObject.SetActive(false);
            
        }
    }

    
    public virtual IEnumerator Shrink()
    {
        yield return new WaitForSeconds(shrinkDelay);

        float percent = 0f;
        float speed = 1f/ shrinkDuration;

        while( percent < 1f)
        {
            percent += Time.deltaTime * speed;

            transform.localScale = Vector3.Lerp(m_startSize, m_shrinkSize, percent);

            yield return null;
        }
    }


}
