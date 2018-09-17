using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

	public Transform target;
	public Transform yawTransform;
	public float sensitivity = 1f;
	public bool invertY = false;
	public Vector2 lookAngle;
	public float smoothRotSpeed;
	public float followDelay = 0.5f;
	public float followMultiplierWhenStationary = 2f;


	PlayerInput m_playerInput;
	Camera m_camera;

	float m_yaw;
	float m_pitch;

	Vector3 smoothRefVelocity;


    // Use this for initialization
    void Start()
    {
		GetComponents();

		lookAngle.x -= Mathf.Abs(m_camera.transform.eulerAngles.x);
		lookAngle.y -= Mathf.Abs(m_camera.transform.eulerAngles.x);
    }

	void GetComponents()
	{
		m_playerInput = FindObjectOfType<PlayerInput>();
		m_camera = FindObjectOfType<Camera>();
	}

	void FixedUpdate()
	{
		UpdateCamRotation();
		UpdateCamPosition();
	}

    void UpdateCamPosition()
    {
		//m_offset = target.position - transform.position;
        //Vector3 desiredPos = transform.position + m_offset;

		transform.position = Vector3.SmoothDamp(transform.position, target.position, ref smoothRefVelocity, followDelay * (m_playerInput.IsMoving() ? 1f : followMultiplierWhenStationary));

    }

    // Update is called once per frame
    void UpdateCamRotation()
    {
		m_yaw -= m_playerInput.MouseV * (invertY ? -1 : 1);
		m_pitch += m_playerInput.MouseH;

		m_yaw = Mathf.Clamp(m_yaw,lookAngle.x, lookAngle.y);

		Debug.Log("angle x: " +lookAngle.x);
		Debug.Log("angle y: " +lookAngle.y);
		Debug.Log("yaw: " +m_yaw);

		Quaternion desiredPitchRotation = Quaternion.Euler(new Vector3(0f,m_pitch,0f));
		Quaternion desiredYawRotation = Quaternion.Euler(new Vector3(m_yaw,0f,0f));

		transform.rotation = Quaternion.Slerp(transform.rotation, desiredPitchRotation , Time.deltaTime * smoothRotSpeed);
		yawTransform.localRotation = Quaternion.Slerp(yawTransform.localRotation, desiredYawRotation, Time.deltaTime * smoothRotSpeed);
    }
}
