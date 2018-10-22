using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{

	public float respawnDelay;

    public event System.Action<int> OnPlayerLoseHealth; // public event our UI is subscribe to so it can change our UI Health base on plyaer current health
    public static event System.Action OnPlayerDeath; // public event our UI is subscribe to so it can change our UI Health base on plyaer current health

    PlayerMovement m_playerMovement;

    Collider[] m_collidersArray;
    Rigidbody m_rigid;

    Rigidbody[] m_childrenRigidsArray;
    Collider[] m_childrenCollidersArray;

	Animator m_anim;

    void GetComponents()
    {
        m_playerMovement = GetComponent<PlayerMovement>();

		m_anim = m_playerMovement.Anim;
        m_rigid = m_playerMovement.Rigid;

        m_collidersArray = GetComponents<Collider>();

        m_childrenRigidsArray = GetComponentsInChildren<Rigidbody>();
        m_childrenCollidersArray = GetComponentsInChildren<Collider>();

		Debug.Log(m_childrenRigidsArray.Length);
		Debug.Log(m_childrenCollidersArray.Length);

        RagdollToggle(false);
    }

    void RagdollToggle(bool state)
    {
        if ( m_childrenCollidersArray != null)
        {
			for(int i = 0; i< m_childrenCollidersArray.Length ; i++)
				m_childrenCollidersArray[i].enabled = state;
		}

		if(m_childrenRigidsArray != null )
		{
            for (int i = 0; i < m_childrenRigidsArray.Length ; i++)
            {
                m_childrenRigidsArray[i].detectCollisions = state;
				m_childrenRigidsArray[i].useGravity = state;
				m_childrenRigidsArray[i].isKinematic = !state;
            }
        }

		m_anim.enabled = !state;

		m_rigid.detectCollisions = !state;
		m_rigid.useGravity = !state;
		m_rigid.isKinematic = state;

		foreach(Collider c in m_collidersArray)
			c.enabled = !state;
    }

    public void Start()
    {
        base.Init();

        GetComponents();

		RagdollToggle(false);
    }

    public override void TakeDamage(int damage)
    {
        CameraShake.isShaking = true; // when we take damage we make our cam Shake
        base.TakeDamage(damage);

        if (OnPlayerLoseHealth != null)
            OnPlayerLoseHealth(m_health); // we invoke this event

        if (m_health <= 0) // if we are dead
        {
            if (OnPlayerDeath != null)
                OnPlayerDeath();

			RagdollToggle(true);
            StartCoroutine(RespawnAfterDelay());
        }
    }

    void Respawn()
    {
        transform.position = CheckpointManager.instance.m_currentCheckpoint.transform.position; // go to last checkpoint that we were in
        m_playerMovement.Rigid.velocity = Vector3.zero; // set velocity to zero
        m_playerMovement.LastMoveVector = Vector3.zero; // and direction to zero so when respawnning player won't keep the speed from before

        m_health = base.startHealth; // reset health
    }

	IEnumerator RespawnAfterDelay()
	{
		yield return new WaitForSeconds(respawnDelay);

		RagdollToggle(false);
		Respawn();

	}

    public void Update()
    {
        if (transform.position.y < -10f) // if we fall below -10f respawn
        {
            if (OnPlayerDeath != null)
                OnPlayerDeath();

            Respawn();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Trap"))
            TakeDamage(1);
    }

}
