using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{

	public float moveSpeed;
	public float smoothTime;
	public float smoothRotateSpeed;
    
	Rigidbody m_rigid;
	PlayerInput m_playerInput;
	CameraController m_CameraController;

	Vector3 moveVector;
	float smoothFactor;
	float smoothVelocityRef;

    void Start()
    {
		GetComponents();
    }

	void GetComponents()
	{
		m_playerInput = GetComponent<PlayerInput>();
		m_rigid = GetComponent<Rigidbody>();
		m_CameraController = FindObjectOfType<CameraController>();
		
	}

    // Update is called once per frame
    void Update()
    {
		CalculateMovePosition();
    }

	void FixedUpdate()
	{
		m_rigid.MovePosition(m_rigid.position + moveVector * Time.fixedDeltaTime);
		UpdateRotation();
	}

	void UpdateRotation()
	{
		Quaternion desiredRot = Quaternion.Slerp(transform.rotation, m_CameraController.transform.rotation, smoothRotateSpeed * Time.fixedDeltaTime);
		m_rigid.MoveRotation(desiredRot);
	}

	void CalculateMovePosition()
	{
		Vector3 vDir = m_playerInput.V * m_CameraController.transform.forward;
		Vector3 hDir = m_playerInput.H * m_CameraController.transform.right;

		Vector3 moveDir = (vDir + hDir).normalized;

		//Vector3 moveDirection = new Vector3(m_playerInput.H, m_rigid.position.y, m_playerInput.V).normalized;
		float moveMagnitude = moveDir.magnitude;

		smoothFactor = Mathf.SmoothDamp(smoothFactor, moveMagnitude, ref smoothVelocityRef, smoothTime );
		
		moveVector = moveDir * moveSpeed * smoothFactor;
	}
}
