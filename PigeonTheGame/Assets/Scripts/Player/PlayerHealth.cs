using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{

	public event System.Action<int> OnPlayerLoseHealth;

	PlayerMovement m_playerMovement;

	public void Start()
	{
		base.Init();
		m_playerMovement = GetComponent<PlayerMovement>();
	}

	public override void TakeDamage(int damage)
	{
		CameraShake.isShaking = true;
		base.TakeDamage(damage);

		if(OnPlayerLoseHealth != null)
			OnPlayerLoseHealth(m_health);

		if(m_health <= 0)
			Respawn();
	}

	void Respawn()
	{
		transform.position = CheckpointManager.instance.m_currentCheckpoint.transform.position;
		m_playerMovement.Rigid.velocity = Vector3.zero;
		m_playerMovement.LastMoveVector = Vector3.zero;

		m_health = base.startHealth;
	}


	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.Return))
		{
			base.TakeDamage(1);
			Debug.Log("Damage");
		}

		if(transform.position.y < -10f || base.IsDead())
		{
			transform.position = CheckpointManager.instance.m_currentCheckpoint.transform.position;
			m_playerMovement.Rigid.velocity = Vector3.zero;
			m_playerMovement.LastMoveVector = Vector3.zero;
		}
	}

}
