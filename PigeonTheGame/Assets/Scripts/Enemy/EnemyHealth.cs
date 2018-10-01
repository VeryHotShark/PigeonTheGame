using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{

    public event System.Action<EnemyHealth> OnEnemyDeath;

    void Start()
    {
        base.Init();
		EnemyManager.instance.Enemies.Add(this);
		EnemyManager.instance.EnemyCount ++;
		OnEnemyDeath += EnemyManager.instance.DecreaseEnemyCount;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if(m_health <= 0)
			Die();
    }

    public override void Die()
    {
        if (OnEnemyDeath != null)
        {
            OnEnemyDeath(this);
        }

        base.Die();
    }

}
