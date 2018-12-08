using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorTrap : MonoBehaviour
{



    public Vector2 minMaxRotation;
    public float rotationDuration;
    public AnimationCurve rotateAnimCurve;

    float m_percent;
    float m_moveSpeed;


    // Use this for initialization
    void Start()
    {
        m_percent = 0f;
        m_moveSpeed = 1f / rotationDuration;
    }

    void Update()
    {


        m_percent += Time.deltaTime * m_moveSpeed;
        var eulerAnglesRot = transform.rotation.eulerAngles;
        eulerAnglesRot.z = Mathf.Lerp(minMaxRotation.x, minMaxRotation.y, rotateAnimCurve.Evaluate(m_percent));
        transform.rotation = Quaternion.Euler(eulerAnglesRot);

    }

}
