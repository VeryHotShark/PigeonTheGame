using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerJumpAnim : MonoBehaviour
{

	Animator m_anim;

	// Use this for initialization
	void Start ()
	{
		m_anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.tag == "Player")
			m_anim.SetTrigger("Jump");
	}
}
