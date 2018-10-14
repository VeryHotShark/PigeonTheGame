using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{

	public int startHealth;

	protected int m_health;

	protected bool m_isDead;

	public virtual void Init()
	{
		m_health = startHealth;
	}

    // Use this for initialization
    public virtual void TakeDamage(int damage)
	{
		m_health -= damage;
	}

	public virtual void TakeDamage(int damage, ContactPoint hitPoint)
	{
		m_health -= damage;
	}
  
	public virtual void Die()
	{
		m_isDead = true;
		Destroy(gameObject);
	}

	public bool IsDead()
	{
		if(m_isDead)
			return true;
		
		return false;
	}

}
