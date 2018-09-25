using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private Rigidbody m_rigid;

    // Use this for initialization
    public void OnProjectileSpawn(Vector3 dir, float force)
    {
		GetComponents();
		m_rigid.AddForce(dir * force, ForceMode.Impulse);
		Destroy(gameObject, 3f);
    }

	void GetComponents()
	{
		m_rigid = GetComponent<Rigidbody>();
	}

    void OnCollisionEnter(Collision other)
    {
        Health otherHealth = other.gameObject.GetComponent<Health>();

        if(otherHealth != null)
        {
            otherHealth.TakeDamage(1);
            Destroy(gameObject);
        }


    }
}
