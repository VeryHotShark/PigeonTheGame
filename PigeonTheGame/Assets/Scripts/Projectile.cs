using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour // TODO zamien to na abstract classe bo będą rózne rodzaje pocisków np gracza , kruków , heavy itp
{
    public GameObject hitVFX;
    public AnimationCurve lifeSizeCurve;

    public bool richochet;
    public bool resizeBullet;

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
    protected AudioSource m_audioSource;
    protected float m_trailStartWidth;

    IEnumerator ResizeBulletRoutine;

    public Vector3 StartSize { get { return m_startSize; } set { m_startSize = value; } }
    protected float TrailStartWidth { get { return m_trailStartWidth; } set { m_trailStartWidth = value; } }

    // Use this for initialization
    public virtual void OnProjectileSpawn(Vector3 dir, float force, int damage, float lifeTime, GameObject objectShotFrom)
    {
        transform.rotation = Quaternion.LookRotation(dir);

        ResetVariables();

        m_startPos = transform.position;
        myForce = force;
        m_speed = force;

        m_dir = dir;

        m_objectShotFrom = objectShotFrom;
        m_damage = damage;
        m_rigid.AddForce(dir * force, ForceMode.Impulse);
        m_distance = lifeTime;
        m_lifeTime = lifeTime;

        if (resizeBullet)
        {
            if (ResizeBulletRoutine != null)
                StopCoroutine(ResizeBulletRoutine);

            ResizeBulletRoutine = SizeOverLifetime();

            StartCoroutine(ResizeBulletRoutine);
        }
    }

    public void GetComponents()
    {
        m_rigid = GetComponent<Rigidbody>();
        m_trailRenderer = GetComponent<TrailRenderer>();
        m_audioSource = GetComponent<AudioSource>();
        m_startSize = transform.localScale;
        if (m_trailRenderer)
            m_trailStartWidth = m_trailRenderer.startWidth;
    }

    public void ResetVariables()
    {
        //Debug.Log(m_startSize);
        transform.localScale = m_startSize;
        if (m_trailRenderer)
        {
            m_trailRenderer.startWidth = m_trailStartWidth;
            m_trailRenderer.startWidth = m_startSize.x - 0.1f;
        }

        m_rigid.velocity = Vector3.zero;
        m_rigid.angularVelocity = Vector3.zero;
    }

    public virtual void OnCollisionEnter(Collision other) // ZMIEN NA RAYCAST, żeby to był projectile zamiast bullet albo pól na pól, że leci sobie i raycast jest na początku Bulletu i on wykrywa zamiast Kolizji
    {

        Health otherHealth = other.gameObject.GetComponent<Health>();

        if (otherHealth == null)
            otherHealth = other.gameObject.GetComponentInParent<Health>();

        if (otherHealth == null && hitVFX != null)
        {

            if (m_audioSource != null)
                m_audioSource.Play();

            /*
              GameObject vfx = Instantiate(hitVFX, other.contacts[0].point, Quaternion.identity);
              vfx.transform.rotation = Quaternion.Euler(other.contacts[0].normal);
              Destroy(vfx, 1f);
             */

            GameObject vfx = VFXPooler.instance.ReuseObject(VFXType.HitProp,other.contacts[0].point,Quaternion.Euler(other.contacts[0].normal));
        }

        if (otherHealth != null && other.gameObject != m_objectShotFrom)
        {
            if (otherHealth.gameObject.GetComponent<EnemyHealth>())
            {
                if (m_objectShotFrom.GetComponent<Enemy>() == null)
                {
                    AudioManager.instance.Play("PlayerHitMark");
                    otherHealth.TakeDamage(m_damage, other.contacts[0]);
                }
            }
            else
                otherHealth.TakeDamage(m_damage, other.contacts[0]);


            if (resizeBullet)
            {
                if (ResizeBulletRoutine != null)
                    StopCoroutine(ResizeBulletRoutine);
            }

            gameObject.SetActive(false);
            //Destroy(gameObject);
        }

        if (richochet)
            ReflectBullet(other);
        else
        {
            if (resizeBullet)
            {
                if (ResizeBulletRoutine != null)
                    StopCoroutine(ResizeBulletRoutine);
            }
            
            gameObject.SetActive(false);
        }
        //Destroy(gameObject);
    }

    void ReflectBullet(Collision other)
    {
        ContactPoint contactPoint = other.contacts[0];

        Vector3 reflectDir = Vector3.Reflect(transform.forward, contactPoint.normal);

        m_rigid.velocity = Vector3.zero;

        //Debug.DrawLine(contactPoint.point,reflectDir,Color.red, 2f);

        float rotation = 90 - Mathf.Atan2(reflectDir.z, reflectDir.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0f, rotation, 0f);
        m_rigid.AddForce(reflectDir * myForce, ForceMode.Impulse);

    }

    public virtual IEnumerator SizeOverLifetime()
    {
        float percent = 0f;
        float speed = 1f / m_lifeTime;

        float desiredSize;

        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            desiredSize = lifeSizeCurve.Evaluate(percent);
            transform.localScale = Vector3.Lerp(m_startSize, Vector3.zero, desiredSize);

            if (m_trailRenderer)
                m_trailRenderer.startWidth = Mathf.Lerp(m_trailStartWidth, 0f, desiredSize);

            yield return null;
        }

        //Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
