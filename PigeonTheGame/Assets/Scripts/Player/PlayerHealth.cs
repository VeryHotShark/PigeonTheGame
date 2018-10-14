using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{

	public event System.Action<int> OnPlayerLoseHealth; // public event our UI is subscribe to so it can change our UI Health base on plyaer current health

	PlayerMovement m_playerMovement;

	public void Start()
	{
		base.Init();
		m_playerMovement = GetComponent<PlayerMovement>();
	}

	public override void TakeDamage(int damage)
	{
		CameraShake.isShaking = true; // when we take damage we make our cam Shake
		base.TakeDamage(damage); 

		if(OnPlayerLoseHealth != null)
			OnPlayerLoseHealth(m_health); // we invoke this event

		if(m_health <= 0) // if we are dead
			Respawn();
	}

	void Respawn()
	{
		transform.position = CheckpointManager.instance.m_currentCheckpoint.transform.position; // go to last checkpoint that we were in
		m_playerMovement.Rigid.velocity = Vector3.zero; // set velocity to zero
		m_playerMovement.LastMoveVector = Vector3.zero; // and direction to zero so when respawnning player won't keep the speed from before

		m_health = base.startHealth; // reset health
	}


	public void Update()
	{
		if(transform.position.y < -10f) // if we fall below -10f respawn
		{
			transform.position = CheckpointManager.instance.m_currentCheckpoint.transform.position;
			m_playerMovement.Rigid.velocity = Vector3.zero;
			m_playerMovement.LastMoveVector = Vector3.zero;
		}
	}

}
