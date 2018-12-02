using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetHinge : MonoBehaviour
{

	Quaternion startRot;
	Rigidbody m_rigid;

    // Use this for initialization
    void Start()
    {
		startRot = transform.rotation;
		m_rigid = GetComponent<Rigidbody>();
		PlayerHealth.OnPlayerRespawn += Reset;
		GameManager.instance.OnGameOver += Unsubscribe;
    }

    // Update is called once per frame
    void Reset()
    {
			transform.rotation = startRot;
			m_rigid.velocity = Vector3.zero;
			m_rigid.angularVelocity = Vector3.zero;
    }

	void Unsubscribe()
	{
		PlayerHealth.OnPlayerRespawn -= Reset;
		GameManager.instance.OnGameOver -= Unsubscribe;
	}
}
