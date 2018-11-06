using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlHealth : EnemyHealth
{

    public override void GetComponents()
    {
        base.GetComponents();

        m_anim = GetComponent<Animator>();

        if (ragdoll)
        {
            m_collider = GetComponentInChildren<Collider>();
            GetRagdollInitTransforms();
            RagdollToggle(false);
        }
    }
}
