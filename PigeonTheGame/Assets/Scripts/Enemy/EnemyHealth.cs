using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{

    public GameObject hitVFX;

    public event System.Action<EnemyHealth> OnEnemyDeath; // public event OnEnemyDeath

    void Start()
    {
        base.Init();
    }

    public override void GetComponents()
    {
        m_anim = GetComponent<Animator>();
        base.GetComponents();

        GetRagdollInitTransforms();
        RagdollToggle(false);
    }

    public override void TakeDamage(int damage, ContactPoint hit)
    {
        GameObject vfx = Instantiate(hitVFX, hit.point, Quaternion.identity);
        vfx.transform.rotation = Quaternion.LookRotation(hit.normal);
        Destroy(vfx, 2f);

        base.TakeDamage(damage);

        if (m_health <= 0)
            Die();
    }

    public override void RagdollToggle(bool state)
    {
        base.RagdollToggle(state);

        m_anim.enabled = !state;
    }

    public override void Die()
    {

        if (OnEnemyDeath != null)
            OnEnemyDeath(this); // call OnEnemyDeath if someone is subscribe to that event

        m_isDead = true;
        RagdollToggle(true);
        //StartCoroutine(RespawnAfterDelay());
    }

}
