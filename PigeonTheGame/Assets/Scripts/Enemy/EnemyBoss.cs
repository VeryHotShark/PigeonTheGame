using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : Enemy
{

    public enum BossStage
    {
        StageOne,
        StageTwo,
        StageThree
    }

    BossStage m_currentStage = BossStage.StageOne;

    [Header("BOSS")]
    [Space]

    public GameObject GFX;

    public float jumpDuration;
    public float jumpHeight;
    public float waitAtPeak;
    public float waitAtGround;
    public float attackDuration;

    bool m_duringRoutine;


    void Start()
    {
        base.GetComponents();
        Init();
        //StartCoroutine(Rise());
    }

    public override void Init()
    {
        m_startPos = transform.position;
        m_starRot = transform.rotation;

        EnemyManager.instance.Enemies.Add(this); // On Start we add this enemy to our EnemyManager.Enemies list
        EnemyManager.instance.EnemyCount++; // and we increment the count of our enemy
        m_health.OnEnemyDeath += EnemyManager.instance.DecreaseEnemyCount; // and we make our EnemyManager to subscribe to our deathEvent so after death EnemyManager will automatically decrease enemies Count
        m_health.OnEnemyDeath += UnsubscribeFromPlayer;
        m_health.OnEnemyDeath += EnemyDied;

        m_playerHealth.OnPlayerLoseHealth += yieldForGivenTime;

        currentState = State.Idle;
        m_health.Init();
    }

    void Update()
    {
        if(RoomManager.instance.PlayerInRoom && RoomManager.instance.PlayerCurrentRoom == roomIndex)
        {
            if(!m_playerHealth.IsDead() && !m_health.IsDead() && m_duringRoutine == false)
            {
                StartCoroutine(Rise());
            }
        }
    }

    IEnumerator Rise()
    {
        GFX.transform.localEulerAngles = Vector3.zero;

        m_duringRoutine = true;

        Vector3 startPos = transform.position;
        Vector3 desiredPos = startPos + Vector3.up * jumpHeight;

        float percent = 0f;
        float speed = 1f / jumpDuration;

        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPos, desiredPos, percent);

            yield return null;
        }

        yield return new WaitForSeconds(waitAtPeak);
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        Vector3 startPos = transform.position;
        Vector3 desiredPos = m_playerTransform.position;

        GFX.transform.localEulerAngles = new Vector3(90f,0f,0f);
        transform.rotation = Quaternion.LookRotation(desiredPos - startPos);

        float percent = 0f;
        float speed = 1f / attackDuration;

        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPos, desiredPos, percent);

            yield return null;
        }

        yield return new WaitForSeconds(waitAtGround);
        StartCoroutine(Rise());
    }

}
