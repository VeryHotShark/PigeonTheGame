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
		EnemyManager.instance.Enemies.Add(this); // On Start we add this enemy to our EnemyManager.Enemies list
		EnemyManager.instance.EnemyCount ++; // and we increment the count of our enemy
		OnEnemyDeath += EnemyManager.instance.DecreaseEnemyCount; // and we make our EnemyManager to subscribe to our deathEvent so after death EnemyManager will automatically decrease enemies Count
    }

    public override void TakeDamage(int damage, ContactPoint hit)
    {
        GameObject vfx = Instantiate(hitVFX, hit.point,Quaternion.identity);
        //Debug.DrawLine(hit.point,hit.normal,Color.red,2f);
        vfx.transform.rotation = Quaternion.LookRotation(hit.normal);
        Destroy(vfx, 2f);

        base.TakeDamage(damage);

        if(m_health <= 0)
			Die();
    }

    public override void Die()
    {
        if (OnEnemyDeath != null)
        {
            OnEnemyDeath(this); // call OnEnemyDeath if someone is subscribe to that event
        }

        base.Die();
    }

}
