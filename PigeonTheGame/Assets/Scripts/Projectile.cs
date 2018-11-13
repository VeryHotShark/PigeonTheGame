using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour // TODO zamien to na abstract classe bo będą rózne rodzaje pocisków np gracza , kruków , heavy itp
{
    public AnimationCurve lifeSizeCurve;

    public bool richochet;

    float lifePercent;

    int m_damage;

    private Rigidbody m_rigid;

    private GameObject m_objectShotFrom;

    float myForce;
    float m_speed;
    float m_distance;
    protected float m_lifeTime;

    Vector3 m_startPos;
    Vector3 m_dir;

    protected Vector3 m_startSize;

    protected TrailRenderer m_trailRenderer;
    protected float m_trailStartWidth;

    // Use this for initialization
    public virtual void OnProjectileSpawn(Vector3 dir, float force, int damage, float lifeTime, GameObject objectShotFrom)
    {
        transform.rotation = Quaternion.LookRotation(dir);

        m_startSize = transform.localScale;
        m_startPos = transform.position;
        myForce = force;
        m_speed = force;

        m_dir = dir;

		GetComponents();
        if(m_trailRenderer)
        {
            m_trailStartWidth = m_trailRenderer.startWidth;
            m_trailRenderer.startWidth = m_startSize.x - 0.1f;
        }

        m_objectShotFrom = objectShotFrom;
        m_damage = damage;
		m_rigid.AddForce(dir * force, ForceMode.Impulse);
        m_distance = lifeTime;
        m_lifeTime = lifeTime;

        StartCoroutine(SizeOverLifetime());
    }

	void GetComponents()
	{
		m_rigid = GetComponent<Rigidbody>();
        m_trailRenderer = GetComponent<TrailRenderer>();
	}
    //void Update()
    //{
        // (transform.position - m_startPos).sqrMagnitude || CHANGE TO THIS LATER TODO zmień sposób liczenia dystansu bo teraz on liczy jak dalkeo jest od spawnPointu a nie ile już drogi przeleciał, ale żeby zrobić żebyśmy znali dystans jaki przemieszcza się co klatkę to trzeba by było albo zrobić kalkulacje jego pozycji tearzniejszej klatki odciąć z poprzednią i tego wyliczyć dystans przebyty i dodać do sumy dystans albo jak zmienić żeby projectile leciał Transform.Translate to wtedy możesz wyliczyć dystans który przybędzie do następnej klatki tak : dystans = speed * Time.deltaTime  
        //Vector3.Distance(m_startPos, transform.position)

        //transform.Translate(transform.InverseTransformDirection(m_dir) * m_speed * Time.deltaTime);

        /*
            if( Vector3.Distance(m_startPos, transform.position) > m_distance)
            {
                Destroy(gameObject);
            }
         */
    //}


    public virtual void OnCollisionEnter(Collision other) // ZMIEN NA RAYCAST, żeby to był projectile zamiast bullet albo pól na pól, że leci sobie i raycast jest na początku Bulletu i on wykrywa zamiast Kolizji
    {
        if(richochet)
            ReflectBullet(other);

        Health otherHealth = other.gameObject.GetComponent<Health>();

        if(otherHealth == null)
            otherHealth = other.gameObject.GetComponentInParent<Health>();

        if(otherHealth != null && other.gameObject != m_objectShotFrom)
        {
            if(otherHealth.gameObject.GetComponent<EnemyHealth>())
            {
                if(m_objectShotFrom.GetComponent<Enemy>() == null)
                    otherHealth.TakeDamage(m_damage,other.contacts[0]);
            }
            else
                otherHealth.TakeDamage(m_damage, other.contacts[0]);

            Destroy(gameObject);
        }

    }

    void ReflectBullet(Collision other)
    {
        ContactPoint contactPoint = other.contacts[0];

        Vector3 reflectDir = Vector3.Reflect(transform.forward, contactPoint.normal);

        m_rigid.velocity = Vector3.zero;

        //Debug.DrawLine(contactPoint.point,reflectDir,Color.red, 2f);

        float rotation = 90 - Mathf.Atan2(reflectDir.z,reflectDir.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0f,rotation, 0f);
        m_rigid.AddForce(reflectDir * myForce, ForceMode.Impulse);

    }

    public virtual IEnumerator SizeOverLifetime()
    {
        float percent = 0f;
        float speed = 1f / m_lifeTime;

        float desiredSize;

        while(percent < 1f)
        {
            percent += Time.deltaTime * speed;
            desiredSize = lifeSizeCurve.Evaluate(percent);
            transform.localScale = Vector3.Lerp(m_startSize, Vector3.zero, desiredSize) ;

            if(m_trailRenderer)
                m_trailRenderer.startWidth = Mathf.Lerp(m_trailStartWidth, 0f, desiredSize);

            yield return null;
        }

        Destroy(gameObject);
    }
}
