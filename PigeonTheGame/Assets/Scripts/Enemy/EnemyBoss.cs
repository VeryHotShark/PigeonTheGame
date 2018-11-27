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

    [Header("VFX")]
    [Space]

    public GameObject hitGroundVFX;
    public GameObject jumpVFX;


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

    [Header("Stage Two")]
    [Space]
    public int shootSeries = 5;
    public int projectileAmountVariation = 2;

    public int projectileAmountAtOnce = 5;
    public float spreadAmount;
    public float waitTimeBeforeNextShootSeries;


    CapsuleCollider m_collider;
    EnemyWeapon m_enemyWeapon;

    bool m_duringRoutine;
    bool m_attackRoutine;
    bool m_inAir;

    int m_wait = Animator.StringToHash("Wait");
    int m_attack = Animator.StringToHash("Attack");
    int m_rise = Animator.StringToHash("Rise");
    int m_shoot = Animator.StringToHash("Shoot");
    int m_shooting = Animator.StringToHash("Shooting");
    int m_secondStage = Animator.StringToHash("SecondStage");

    public override void Init()
    {
        if (!m_health.IsDead())
        {
            EnemyManager.instance.Enemies.Add(this); // On Start we add this enemy to our EnemyManager.Enemies list
            EnemyManager.instance.EnemyCount++; // and we increment the count of our enemy

            m_health.OnEnemyDeath += EnemyManager.instance.DecreaseEnemyCount; // and we make our EnemyManager to subscribe to our deathEvent so after death EnemyManager will automatically decrease enemies Count
            m_health.OnEnemyDeath += UnsubscribeFromPlayer;
            m_health.OnEnemyDeath += EnemyDied;
            m_health.OnEnemyHalfHealth += ChangeState;
            m_health.OnBossDeath += ChangeGameState;

            m_playerHealth.OnPlayerLoseHealth += yieldForGivenTime;
            PlayerHealth.OnPlayerRespawn += ResetVariables;
        }

        StopAllCoroutines();

        m_currentStage = BossStage.StageOne;
        m_duringRoutine = false;


        currentState = State.Idle;
        m_health.Init();

        m_collider.enabled = true;

    }

    public override void GetComponents()
    {
        base.GetComponents();
        m_enemyWeapon = GetComponent<EnemyWeapon>();
        m_collider = GetComponentInChildren<CapsuleCollider>();
    }

    void Update()
    {


        if (RoomManager.instance.PlayerInRoom && RoomManager.instance.PlayerCurrentRoom == roomIndex)
        {
            if (!m_playerHealth.IsDead() && !m_health.IsDead())
            {
                switch (m_currentStage)
                {
                    case BossStage.StageOne:
                        {
                            if (!m_duringRoutine)
                                StartCoroutine(Rise());
                        }
                        break;

                        case BossStage.StageTwo:
                        {
                        FaceTarget();
                        //if (!m_duringRoutine)
                        //StartCoroutine(ShootSeries());
                        }
                        break;
                }
            }
        }
    }

    void ChangeState()
    {
        if (m_currentStage == BossStage.StageOne)
            m_currentStage = BossStage.StageTwo;
    }

    void ChangeGameState()
    {
        GameManager.instance.GameIsOver = true;
        GameManager.instance.Boss = transform;
        
        GameManager.instance.InvokeEvent();
    }

    IEnumerator Rise()
    {
        AudioManager.instance.PlayClipAt("OwlJump", transform.position);

        GFX.transform.localPosition = Vector3.zero;
        FaceTarget();

        if(m_currentStage == BossStage.StageOne)
            m_attackRoutine = true;
        m_duringRoutine = true;
        m_inAir = true;

        m_collider.direction = 1;
        //transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);

        m_anim.SetTrigger(m_rise);
        yield return new WaitForSeconds(0.4f);

        GameObject jumpInstance = VFXPooler.instance.ReuseObject(VFXType.OwlJump,transform.position,Quaternion.identity);

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



        if (m_currentStage == BossStage.StageOne || m_attackRoutine == true)
            StartCoroutine(Attack());
        else
        {
            StartCoroutine(ShootSeries());
        }
    }


    IEnumerator Attack()
    {
        FaceTarget();

        AudioManager.instance.PlayClipAt("OwlAttack", transform.position);

        m_collider.direction = 2;
        m_anim.SetBool(m_attack, true);

        Vector3 startPos = transform.position;
        Vector3 desiredPos = m_playerTransform.position;

        //GFX.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        //GFX.transform.localRotation = Quaternion.LookRotation(desiredPos - startPos);
        GFX.transform.localPosition = localGFXOffset;

        float percent = 0f;
        float speed = 1f / attackDuration;

        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPos, desiredPos, percent);

            if(percent < 0.65f)
            {
                FaceTarget();
                desiredPos = m_playerTransform.position;
            }

            yield return null;
        }

        AudioManager.instance.PlayClipAt("OwlHitGround", transform.position);

    /*
        GameObject vfx = Instantiate(hitGroundVFX, transform.position, Quaternion.identity);
        Destroy(vfx, 3f);
     */
        GameObject vfx = VFXPooler.instance.ReuseObject(VFXType.HitGround,transform.position ,Quaternion.identity);

        yield return new WaitForSeconds(waitAtGround);

        //GFX.transform.localEulerAngles = Vector3.zero;
        //GFX.transform.localPosition = Vector3.zero;

        m_anim.SetBool(m_attack, false);
        m_inAir = false;


        if (m_currentStage == BossStage.StageTwo)
        {
            m_attackRoutine = false;
            m_anim.SetBool(m_secondStage, true);
        }

        /*    
            if (m_currentStage == BossStage.StageTwo)
            {
                m_attackRoutine = false;
                m_duringRoutine = false;
                yield break;
            }
         */

        StartCoroutine(Rise());
    }

    IEnumerator ShootSeries()
    {
        m_anim.SetBool(m_shooting, true);

        currentState = State.Attack;
        m_duringRoutine = true; // set duringRoutine to true


        int randomShootSeries = Random.Range(shootSeries - 1, shootSeries + 2);

        while (randomShootSeries > 0 && m_playerRested) // while amount to shoot is greater than 0
        {
            FaceTarget();
            m_anim.SetTrigger(m_shoot);
            AudioManager.instance.PlayClipAt("OwlShoot", transform.position);

            int amountToShoot = Random.Range(projectileAmountAtOnce - projectileAmountVariation, projectileAmountAtOnce + projectileAmountVariation + 1); // calculate how many projectiles will be shot

            for (int i = 0; i < amountToShoot; i++)
            {
                Vector3 randomPoint;

                if(i == 0)
                {
                    randomPoint = m_playerTransform.position;
                }
                else
                {

                    randomPoint = m_playerTransform.position + Random.insideUnitSphere * spreadAmount;
                }

                m_enemyWeapon.ShootProjectile(randomPoint);

                yield return null;
            }

            //m_enemyWeapon.ShootProjectile(m_playerTransform.position); // we spawn projectile
            randomShootSeries--; // we decrease amountToShoot by one
            yield return new WaitForSeconds(attackRate); // and we wait for some time between shots
        }

        StartCoroutine(WaitIdle()); // after shootSeries we can optionally wait for few seconds ex. for enemy to reload or something
    }

    IEnumerator WaitIdle()
    {
        //m_duringRoutine = false; // set our bool to false
        currentState = State.Idle; // change our state to idle
        m_anim.SetBool(m_wait, true);

        yield return new WaitForSeconds(waitTimeBeforeNextShootSeries); // wait for some delay

        m_anim.SetBool(m_wait, false);
        m_anim.SetBool(m_attack, true);

        StartCoroutine(Attack());


        /*  
            if(m_inAir)
                StartCoroutine(Attack());
            else
                StartCoroutine(Rise());
         */
    }

    public override void ResetVariables()
    {
        base.ResetVariables();

        GFX.transform.localPosition = Vector3.zero;
        m_collider.direction = 1;
    }



}
