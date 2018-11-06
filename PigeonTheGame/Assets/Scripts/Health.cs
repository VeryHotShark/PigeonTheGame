using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Health : MonoBehaviour
{
    public bool ragdoll;
    public int startHealth;

    protected int m_health;
    public int CurrentHealth {get{return m_health;}}

    protected bool m_isDead;

    protected Animator m_anim;

    // RAGDOLL

   

    protected List<Rigidbody> m_childrenRigidsList;
    protected Collider[] m_childrenCollidersArray;

    protected List<Vector3> m_ragdollStartLocalPos = new List<Vector3>();
    protected List<Quaternion> m_ragdollStartLocalRot = new List<Quaternion>();


    public virtual void Init()
    {
        m_health = startHealth;
        m_isDead = false;
    }

    public virtual void GetComponents()
    {
        if (ragdoll)
        {
            m_childrenRigidsList = GetComponentsInChildren<Rigidbody>().ToList();
            m_childrenCollidersArray = GetComponentsInChildren<Collider>();
        }

    }

    // Use this for initialization
    public virtual void TakeDamage(int damage)
    {
        m_health -= damage;
    }

    public virtual void TakeDamage(int damage, ContactPoint hitPoint)
    {
        m_health -= damage;
    }

    public virtual void Die()
    {
        m_isDead = true;
        gameObject.SetActive(false);
    }

    public bool IsDead()
    {
        if (m_isDead)
            return true;

        return false;
    }

    public virtual void GetRagdollInitTransforms()
    {

        for (int i = 0; i < m_childrenRigidsList.Count; i++)
        {
            Transform ragdollTransform = m_childrenRigidsList[i].transform;

            m_ragdollStartLocalPos.Add(ragdollTransform.localPosition);
            m_ragdollStartLocalRot.Add(ragdollTransform.localRotation);
        }
    }

    public virtual void ResetRagdollTransform()
    {
        for (int i = 0; i < m_ragdollStartLocalPos.Count; i++)
        {
            Transform ragdollTransform = m_childrenRigidsList[i].transform;

            ragdollTransform.localPosition = m_ragdollStartLocalPos[i];
            ragdollTransform.localRotation = m_ragdollStartLocalRot[i];

            m_childrenRigidsList[i].transform.localPosition = ragdollTransform.localPosition;
            m_childrenRigidsList[i].transform.localRotation = ragdollTransform.localRotation;

        }
    }

    public virtual void RagdollToggle(bool state)
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

        //m_anim.enabled = !state;

        //foreach (Collider c in m_collidersArray)
        //    c.enabled = !state;
    }

}
