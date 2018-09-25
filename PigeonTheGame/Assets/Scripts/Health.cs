using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{

	public int startHealth;

	protected int m_health;

	public virtual void Init()
	{
		m_health = startHealth;
	}

    // Use this for initialization
    public virtual void TakeDamage(int damage)
	{
		m_health -= damage;

		if(m_health <= 0)
			Die();
	}
  
	public virtual void Die()
	{
		Destroy(gameObject);
	}

}
