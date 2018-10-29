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
    public Vector3 localGFXOffset;

    [Space]

    public float jumpDuration;
    public float jumpHeight;
    public float waitAtPeak;
    public float waitAtGround;
    public float attackDuration;


    CapsuleCollider m_collider;

    bool m_duringRoutine;

    int m_wait = Animator.StringToHash("Wait");
    int m_attack = Animator.StringToHash("Attack");
    int m_rise = Animator.StringToHash("Rise");

    void Start()
    {
        GetComponents();
        Init();
        //StartCoroutine(Rise());
    }

    public override void Init()
    {

        EnemyManager.instance.Enemies.Add(this); // On Start we add this enemy to our EnemyManager.Enemies list
        EnemyManager.instance.EnemyCount++; // and we increment the count of our enemy

        m_health.OnEnemyDeath += EnemyManager.instance.DecreaseEnemyCount; // and we make our EnemyManager to subscribe to our deathEvent so after death EnemyManager will automatically decrease enemies Count
        m_health.OnEnemyDeath += UnsubscribeFromPlayer;

        m_playerHealth.OnPlayerLoseHealth += yieldForGivenTime;

        currentState = State.Idle;
        m_health.Init();

    }

    public override void GetComponents()
    {
        base.GetComponents();
        m_collider = GetComponentInChildren<CapsuleCollider>();
    }

    void Update()
    {
        if (RoomManager.instance.PlayerInRoom && RoomManager.instance.PlayerCurrentRoom == roomIndex)
        {
            if (!m_playerHealth.IsDead() && !m_health.IsDead() && m_duringRoutine == false)
            {
                StartCoroutine(Rise());
            }
        }
    }

    IEnumerator Rise()
    {
        m_collider.direction = 1;
        transform.localEulerAngles = new Vector3(0f,transform.localEulerAngles.y,0f);

        m_anim.SetTrigger(m_rise);
        yield return new WaitForSeconds(0.4f);

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

        m_anim.SetBool(m_wait, true);
        yield return new WaitForSeconds(waitAtPeak);
        m_anim.SetBool(m_wait, false);

        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        m_collider.direction = 2;
        m_anim.SetBool(m_attack, true);

        Vector3 startPos = transform.position;
        Vector3 desiredPos = m_playerTransform.position;

        //GFX.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        GFX.transform.rotation = Quaternion.LookRotation(desiredPos - startPos);
        GFX.transform.localPosition = localGFXOffset;

        float percent = 0f;
        float speed = 1f / attackDuration;

        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPos, desiredPos, percent);

            yield return null;
        }



        yield return new WaitForSeconds(waitAtGround);

        GFX.transform.localEulerAngles = Vector3.zero;
        GFX.transform.localPosition = Vector3.zero;
        
        m_anim.SetBool(m_attack, false);

        StartCoroutine(Rise());
    }

}
