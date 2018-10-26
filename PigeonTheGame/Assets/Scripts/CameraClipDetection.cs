using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClipDetection : MonoBehaviour
{

    public LayerMask layersToCollideWith;

	public float zoomAmount;
    public float moveSpeed;
    public float returnSpeed;
    public float maxZoomDistance;
	public float collisionPadding = 0.5f;


    CameraController m_camController;
    Transform m_parentTransform;
    Transform m_target;
    Vector3 m_initPos;
    Vector3 m_dirFromParentToCam;

    Ray m_ray = new Ray();
    RaycastHit[] m_hits;

    float m_initialDst;
    float m_currentDst;
    float m_nearestDst;

    bool m_clip;


    // Use this for initialization
    void Start()
    {
        Init();
    }

    void Init()
    {
        m_camController = transform.root.GetComponent<CameraController>();
        m_target = FindObjectOfType<PlayerMovement>().transform;
        m_parentTransform = transform.parent;
        m_initPos = transform.localPosition;

		m_dirFromParentToCam = (transform.position - m_parentTransform.position).normalized;

        m_initialDst = transform.localPosition.magnitude; // Same as calculating Vector3.Distance between thjis transform and parent transform;
        m_currentDst = m_initialDst;
		m_nearestDst = m_currentDst;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CheckForCollision();
    }

    void CheckForCollision()
    {
		m_dirFromParentToCam = (transform.position - m_parentTransform.position).normalized;

		//Debug.DrawLine(m_parentTransform.position,transform.position,Color.blue,2f);
		//Debug.DrawLine(m_parentTransform.position,m_parentTransform.position + m_dirFromParentToCam * 5f ,Color.green,2f);

        m_currentDst = transform.localPosition.magnitude;

        m_ray = new Ray(m_parentTransform.position, m_dirFromParentToCam);
        RaycastHit hit;

        bool hitSomething = Physics.Raycast(m_ray, out hit, 50f, layersToCollideWith, QueryTriggerInteraction.Ignore);

        if (hitSomething)
        {
            if (hit.distance < m_initialDst && hit.distance - 1f <= m_nearestDst)
            {
                m_clip = true;
				//Debug.Log(hit.collider.transform.name);
                m_nearestDst = hit.distance;
            }
            else
            {
                m_clip = false;
				//Debug.Log("Falsz1");
                m_nearestDst = m_currentDst;
            }
        }
        else
        {
            m_clip = false;
			Debug.Log("Falsz2");
            m_nearestDst = m_currentDst;
        }

		if(m_clip)
		{
			Vector3 dirToPlayer = (m_parentTransform.position - hit.point).normalized;
			Vector3 desiredPos =  m_parentTransform.InverseTransformPoint(hit.point) + m_parentTransform.InverseTransformDirection(dirToPlayer) * zoomAmount;

			Debug.DrawLine(m_ray.origin,hit.point,Color.red,2f);
			Debug.DrawLine(hit.point,hit.point + dirToPlayer * collisionPadding,Color.black,2f);
			//desiredPos = new Vector3(transform.localPosition.x,transform.localPosition.y,desiredPos.z);
			//Debug.Log(desiredPos.magnitude);

			if(desiredPos.magnitude > maxZoomDistance)
				transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPos, moveSpeed * Time.deltaTime);
			//else
			//	transform.localPosition = hit.point;
		}
		else if(m_clip == false && m_camController.ZoomingFinish == true)
		{
			transform.localPosition = Vector3.Lerp(transform.localPosition, m_initPos, returnSpeed * Time.deltaTime);
		}

		//Debug.Log(m_clip);
    }
}
