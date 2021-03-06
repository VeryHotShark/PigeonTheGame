﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{

    public bool GodMode;
    public bool DeathMode;

    public GameObject hitVFX;
    public GameObject healVFX;
    public float respawnDelay;

    public event System.Action<int> OnPlayerLoseHealth; // public event our UI is subscribe to so it can change our UI Health base on plyaer current health
    public event System.Action OnPlayerReachCheckPoint;
    public static event System.Action OnPlayerDeath;
    public static event System.Action OnPlayerRespawn;
    public static event System.Action OnPlayerBigDeath;

    protected Collider[] m_collidersArray;

    PlayerMovement m_playerMovement;
    Rigidbody m_rigid;

    bool m_justGotHit;

    int m_hitHash = Animator.StringToHash("Hit");

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
        if (m_isDead)
            return;

        CameraShake.isShaking = true; // when we take damage we make our cam Shake
        base.TakeDamage(damage);

        AudioManager.instance.Play("PlayerHit");
        m_playerMovement.Anim.SetTrigger(m_hitHash);

    /*
        GameObject playerHitVFX = Instantiate(hitVFX, transform.position + Vector3.up, Quaternion.identity);
        Destroy(playerHitVFX, 0.3f);
     */
        GameObject vfx = VFXPooler.instance.ReuseObject(VFXType.Hit,transform.position + Vector3.up,Quaternion.identity);

        if (!GodMode)
        {
            if (OnPlayerLoseHealth != null)
                OnPlayerLoseHealth(m_health); // we invoke this event

            if (m_health <= 0)
            {
                Die();
            }

        }
    }

    public override void TakeDamage(int damage, ContactPoint point)
    {
        if (m_isDead)
            return;

        CameraShake.isShaking = true; // when we take damage we make our cam Shake
        base.TakeDamage(damage);

        AudioManager.instance.Play("PlayerHit");
        m_playerMovement.Anim.SetTrigger(m_hitHash);

    /*
        GameObject playerHitVFX = Instantiate(hitVFX, point.point, Quaternion.identity);
        Destroy(playerHitVFX, 0.3f);
     */

        GameObject vfx = VFXPooler.instance.ReuseObject(VFXType.Hit,point.point ,Quaternion.identity);

        if (!GodMode)
        {

            if (OnPlayerLoseHealth != null)
                OnPlayerLoseHealth(m_health); // we invoke this event

            if (m_health <= 0)
            {
              Die();
            }
        }
    }

    public override void Die()
    {
        m_isDead = true;

        if (OnPlayerDeath != null)
            OnPlayerDeath();

        RagdollToggle(true);
        StartCoroutine(RespawnAfterDelay());
    }

    void Respawn()
    {
        m_isDead = false;
        gameObject.transform.parent = null;

        if (OnPlayerRespawn != null)
            OnPlayerRespawn();

        transform.position = CheckpointManager.instance.m_currentCheckpoint.transform.position; // go to last checkpoint that we were in
        //transform.rotation = CheckpointManager.instance.m_currentCheckpoint.transform.localRotation;
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
         if (Input.GetKeyDown(KeyCode.T) && DeathMode) // if we fall below -10f respawn
         {
            TakeDamage(3);
         }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Trap"))
            TakeDamage(3);

        if (other.gameObject.layer == LayerMask.NameToLayer("SpinningTrap") && !m_justGotHit)
        {
            TakeDamage(1);
            m_justGotHit = true;

            StartCoroutine(ImmuneDuration());
        }

        
        if (other.gameObject.layer == LayerMask.NameToLayer("AnchorTrap") && !m_justGotHit)
        {
            TakeDamage(1);
            m_justGotHit = true;

            StartCoroutine(ImmuneDuration());
        }
    }

    void OnTriggerExit(Collider other)
    {
        RoomTrigger roomTrigger = other.GetComponent<RoomTrigger>();

        if (roomTrigger != null)
        {
            if (m_health != startHealth && roomTrigger.HealthResetted == false && roomTrigger.healthReset)
            {
                roomTrigger.HealthResetted = true;
                m_health = startHealth;

                if (OnPlayerReachCheckPoint != null)
                    OnPlayerReachCheckPoint();

                AudioManager.instance.Play("PlayerHeal");

            /*
                GameObject healVFXInstance = Instantiate(healVFX, transform.position + Vector3.up, Quaternion.identity);
                healVFXInstance.transform.parent = transform;
                Destroy(healVFXInstance, 1.5f);
             */

                GameObject vfx = VFXPooler.instance.ReuseObject(VFXType.Heal,transform.position + Vector3.up ,Quaternion.identity);
                vfx.transform.parent = transform;
            }
        }
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
            if (!other.gameObject.GetComponentInParent<EnemyHealth>().IsDead() && !m_justGotHit)
            {
                m_justGotHit = true;
                TakeDamage(1);
                StartCoroutine(ImmuneDuration());
                //m_playerMovement.Rigid.AddForce(-m_playerMovement.transform.forward * 20f, ForceMode.Impulse);
            }
        }

        
        if (other.gameObject.layer == LayerMask.NameToLayer("SpinningTrap") && !m_justGotHit)
        {
            TakeDamage(1);
            m_justGotHit = true;

            StartCoroutine(ImmuneDuration());
        }


    }

    WaitForSeconds yieldImmune = new WaitForSeconds(0.2f);

    IEnumerator ImmuneDuration()
    {
        yield return yieldImmune;

        m_justGotHit = false;
    }


}
