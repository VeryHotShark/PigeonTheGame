using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{

    public float respawnDelay;

    public event System.Action<int> OnPlayerLoseHealth; // public event our UI is subscribe to so it can change our UI Health base on plyaer current health
    public static event System.Action OnPlayerDeath;
    public static event System.Action OnPlayerRespawn;

    PlayerMovement m_playerMovement;

    Rigidbody m_rigid;
    public void Start()
    {
        base.Init();

        GetComponents();
    }

    public override void GetComponents()
    {
        m_playerMovement = GetComponent<PlayerMovement>();

        m_anim = m_playerMovement.Anim;
        m_rigid = m_playerMovement.Rigid;

        if (ragdoll)
        {
            m_collidersArray = GetComponents<Collider>();

            m_childrenRigidsList = GetComponentsInChildren<Rigidbody>().ToList();
            m_childrenCollidersArray = GetComponentsInChildren<Collider>();

            RemoveParentRigidFromList();
            GetRagdollInitTransforms();

            RagdollToggle(false);
        }
    }

    void RemoveParentRigidFromList()
    {

        for (int i = 0; i < m_childrenRigidsList.Count; i++)
        {
            if (m_rigid == m_childrenRigidsList[i])
            {
                m_childrenRigidsList.Remove(m_childrenRigidsList[i]);
                return;
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        CameraShake.isShaking = true; // when we take damage we make our cam Shake
        base.TakeDamage(damage);

        AudioManager.instance.Play("PlayerHit");

        if (OnPlayerLoseHealth != null)
            OnPlayerLoseHealth(m_health); // we invoke this event

        if (m_health <= 0) // if we are dead
            Die();
    }

    public override void Die()
    {
        if (OnPlayerDeath != null)
            OnPlayerDeath();

        m_isDead = true;
        RagdollToggle(true);
        StartCoroutine(RespawnAfterDelay());
    }

    void Respawn()
    {
        m_isDead = false;

        if (OnPlayerRespawn != null)
            OnPlayerRespawn();

        transform.position = CheckpointManager.instance.m_currentCheckpoint.transform.position; // go to last checkpoint that we were in
        m_playerMovement.Rigid.velocity = Vector3.zero; // set velocity to zero
        m_playerMovement.LastMoveVector = Vector3.zero; // and direction to zero so when respawnning player won't keep the speed from before

        m_health = base.startHealth; // reset health
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        RagdollToggle(false);
        ResetRagdollTransform();
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
            TakeDamage(3);
    }

    public override void RagdollToggle(bool state)
    {
        if (m_childrenCollidersArray != null)
        {
            for (int i = 0; i < m_childrenCollidersArray.Length; i++)
                m_childrenCollidersArray[i].enabled = state;
        }

        if (m_childrenRigidsList != null)
        {
            for (int i = 0; i < m_childrenRigidsList.Count; i++)
            {
                m_childrenRigidsList[i].detectCollisions = state;
                m_childrenRigidsList[i].useGravity = state;
                m_childrenRigidsList[i].isKinematic = !state;
            }
        }

        m_anim.enabled = !state;
        m_rigid.detectCollisions = !state;
        m_rigid.useGravity = !state;
        m_rigid.isKinematic = state;

        foreach (Collider c in m_collidersArray)
            c.enabled = !state;
    }


    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Owl"))
        {
            TakeDamage(1);
            m_playerMovement.Rigid.AddForce(-m_playerMovement.transform.forward * 20f, ForceMode.Impulse);
        }

    }

}
