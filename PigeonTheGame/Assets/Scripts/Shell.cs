using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{

	public Vector2 minMaxForce;
	Rigidbody m_rigid;

    // Use this for initialization
    void Start()
    {
		m_rigid = GetComponent<Rigidbody>();
		float randomForce = Random.Range(minMaxForce.x,minMaxForce.y);
		m_rigid.AddForce(transform.right * randomForce, ForceMode.Impulse);
		m_rigid.AddTorque(Random.insideUnitSphere * randomForce);
    }

}
