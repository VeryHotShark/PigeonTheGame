using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    int m_damage;

    private Rigidbody m_rigid;

    private GameObject m_objectShotFrom;

    float myForce;

    // Use this for initialization
    public void OnProjectileSpawn(Vector3 dir, float force, int damage, float lifeTime, GameObject objectShotFrom)
    {
        myForce = force;

		GetComponents();
        m_objectShotFrom = objectShotFrom;
        m_damage = damage;
		m_rigid.AddForce(dir * force, ForceMode.Impulse);
		Destroy(gameObject, lifeTime);
    }

	void GetComponents()
	{
		m_rigid = GetComponent<Rigidbody>();
	}

    void OnCollisionEnter(Collision other) // ZMIEN NA RAYCAST, żeby to był projectile zamiast bullet albo pól na pól, że leci sobie i raycast jest na początku Bulletu i on wykrywa zamiast Kolizji
    {
        
        ContactPoint contactPoint = other.contacts[0];

        Vector3 reflectDir = Vector3.Reflect(transform.forward, contactPoint.normal);

        m_rigid.velocity = Vector3.zero;

        //Debug.DrawLine(contactPoint.point,reflectDir,Color.red, 2f);

        float rotation = 90 - Mathf.Atan2(reflectDir.z,reflectDir.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0f,rotation, 0f);
        m_rigid.AddForce(reflectDir * myForce, ForceMode.Impulse);

        Health otherHealth = other.gameObject.GetComponent<Health>();

        if(otherHealth != null && other.gameObject != m_objectShotFrom)
        {
            if(otherHealth.gameObject.GetComponent<EnemyHealth>())
            {
                if(m_objectShotFrom.GetComponent<Enemy>() == null)
                    otherHealth.TakeDamage(m_damage,other.contacts[0]);
            }
            else
                otherHealth.TakeDamage(m_damage);

            Destroy(gameObject);
        }
    }
}
