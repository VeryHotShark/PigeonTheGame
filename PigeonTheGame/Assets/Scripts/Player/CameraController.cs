using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

	public enum CameraType
	{
		SmoothFollow,
		SnapCamera
	};

	public CameraType cameraType;

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

	public Camera GetCamera { get { return m_camera;}}

	float m_yaw;
	float m_pitch;

	Vector3 smoothRefVelocity;

	Vector3 m_offset;

    // Use this for initialization
    void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		GetComponents();

		// WE SUBSTRACT INIT CAMERA ROTATION FROM MIN-MAX ANGLE

		lookAngle.x -= Mathf.Abs(m_camera.transform.eulerAngles.x);
		lookAngle.y -= Mathf.Abs(m_camera.transform.eulerAngles.x);
    }

	void GetComponents()
	{
		m_playerInput = FindObjectOfType<PlayerInput>();
		m_camera = FindObjectOfType<Camera>();
	}

	void LateUpdate()
	{
		if(cameraType == CameraType.SnapCamera)
			UpdateCamPosition();
	}

	void FixedUpdate()
	{
		UpdateCamRotation();

		if(cameraType == CameraType.SmoothFollow)
			UpdateCamPosition();
	}

    void UpdateCamPosition()
    {
		if(target == null) // IF THERE IS NO TARGET , RETURN
			return;

		m_offset = target.position - transform.position; // offset between player and our position
        Vector3 desiredPos = transform.position + m_offset; // desired position that we want to be

		if(cameraType == CameraType.SmoothFollow)
			transform.position = Vector3.SmoothDamp(transform.position, target.position, ref smoothRefVelocity, followDelay * (m_playerInput.NoInput() ? 1f : followMultiplierWhenStationary)); // smooth our camera
		else
			transform.position = target.position; // snap our camera

    }

    // Update is called once per frame
    void UpdateCamRotation()
    {
		// Calculate our yaw and pitch variable based on mouse input

		m_yaw -= m_playerInput.MouseV * (invertY ? -1 : 1);
		m_pitch += m_playerInput.MouseH;

		m_yaw = Mathf.Clamp(m_yaw,lookAngle.x, lookAngle.y); // we clamp yaw value so we cant rotate fully 360 around x axis

		//Debug.Log("angle x: " +lookAngle.x);
		//Debug.Log("angle y: " +lookAngle.y);
		//Debug.Log("yaw: " +m_yaw);


		// Create a Quaternion rotation for our yaw and pitch rotation

		Quaternion desiredPitchRotation = Quaternion.Euler(new Vector3(0f,m_pitch,0f));
		Quaternion desiredYawRotation = Quaternion.Euler(new Vector3(m_yaw,0f,0f));

		if(cameraType == CameraType.SmoothFollow)
		{
			// Smooth our rotation
			
			transform.rotation = Quaternion.Slerp(transform.rotation, desiredPitchRotation , Time.deltaTime * smoothRotSpeed);
			yawTransform.localRotation = Quaternion.Slerp(yawTransform.localRotation, desiredYawRotation, Time.deltaTime * smoothRotSpeed);
		}
		else
		{
			// Snap our rotation

			transform.rotation = desiredPitchRotation;
			yawTransform.localRotation = desiredYawRotation;
		}
    }
}
