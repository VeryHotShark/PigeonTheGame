using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    public GameObject hitVFX;
    public GameObject deathVFX;

    public event System.Action<EnemyHealth> OnEnemyDeath; // public event OnEnemyDeath
    public event System.Action OnEnemyHalfHealth; // public event OnEnemyDeath

    public static event System.Action OnEnemyTakeDamage; // public event OnEnemyDeath
    public static event System.Action OnAnyEnemyDeath; // public event OnEnemyDeath

    protected Collider m_collider;

    public override void Init()
    {
        base.Init();

        PlayerHealth.OnPlayerRespawn += Deactivate;
    }

    public override void GetComponents()
    {
        base.GetComponents();

        m_anim = GetComponent<Animator>();

        if (ragdoll)
        {
            m_collider = GetComponent<Collider>();
            GetRagdollInitTransforms();
            RagdollToggle(false);
        }

    }

    public override void TakeDamage(int damage, ContactPoint hit)
    {
        if (hitVFX != null)
        {
            GameObject vfx = Instantiate(hitVFX, hit.point, Quaternion.identity);
            vfx.transform.rotation = Quaternion.LookRotation(hit.normal);
            Destroy(vfx, 2f);
        }

        if (OnEnemyTakeDamage != null)
            OnEnemyTakeDamage();

        AudioManager.instance.PlayClipAt("EnemyHit", transform.position);

        if (CurrentHealth < Mathf.RoundToInt(startHealth / 2f))
            if (OnEnemyHalfHealth != null)
                OnEnemyHalfHealth();

        base.TakeDamage(damage);

        if (m_health <= 0)
        {
            if (deathVFX != null)
            {
                GameObject vfxInstance = Instantiate(deathVFX, hit.point, Quaternion.identity);
                vfxInstance.transform.rotation = Quaternion.LookRotation(-hit.normal);
                Destroy(vfxInstance, 2f);
            }
            Die();
        }
    }

    public override void RagdollToggle(bool state)
    {
        base.RagdollToggle(state);

        if (m_collider != null)
        {
            m_collider.enabled = !state;
        }
        else
        {
            m_collider = GetComponentInChildren<Collider>();
            m_collider.enabled = !state;
        }

        if (m_anim != null)
            m_anim.enabled = !state;
    }

    public override void Die()
    {
        if (OnAnyEnemyDeath != null)
            OnAnyEnemyDeath(); // call OnEnemyDeath if someone is subscribe to that event


        if (OnEnemyDeath != null)
            OnEnemyDeath(this); // call OnEnemyDeath if someone is subscribe to that event

        m_isDead = true;

        if (ragdoll)
            RagdollToggle(true);
        else
            gameObject.SetActive(false);
    }

    public void Deactivate()
    {
        if (m_isDead)
            gameObject.SetActive(false);

        PlayerHealth.OnPlayerRespawn -= Deactivate;
    }


}
