using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{

	[Header("Player Camera")]
	public bool snapToCamera;
	public float smoothRotateSpeed;

	[Space]
	[Header("Player Movement")]
	public float moveSpeed;
	public float smoothTime;
	public float turnSpeed;
	public float dashPower = 5f;

	[Space]
	[Header("Player Jump")]
	public float jumpPower = 10f;
	public float gravityMultiplier = 2f;
	public float lowFallMultiplier = 3f;
	public LayerMask groundLayerMask;



    
	Rigidbody m_rigid;
	PlayerInput m_playerInput;
	CameraController m_CameraController;

	Vector3 m_moveVector;
	Vector3 m_moveDir;
	Vector3 m_currentMoveDir;
	Vector3 m_lastMoveDir;
	float m_smoothFactor;
	float m_smoothVelocityRef;

	float m_angle;

	bool m_isGrounded;

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
		CheckIfGrounded();
		PlayerDash();
		PlayerJump();
    }

	void FixedUpdate()
	{
		CalculateMovePosition();
		m_rigid.MovePosition(m_rigid.position + (m_playerInput.NoInput() && !m_isGrounded ? m_lastMoveDir / 2f : m_moveVector));
		//m_rigid.velocity = m_moveVector;
		UpdateRotation();
	}

	void UpdateRotation()
	{
		Quaternion desiredRot = m_CameraController.transform.rotation;

		if(!snapToCamera)
			desiredRot = Quaternion.Slerp(transform.rotation, m_CameraController.transform.rotation, smoothRotateSpeed * Time.fixedDeltaTime);

		m_rigid.MoveRotation(desiredRot);
	}

	void CheckIfGrounded()
	{
		Ray ray = new Ray(transform.position, Vector3.down);
		RaycastHit hit;

		if(Physics.Raycast(ray,out hit,0.6f,groundLayerMask))
		{
			m_isGrounded = true;
		}
		else
		{
			m_isGrounded = false;
		}
	}

	void PlayerDash()
	{
		if(m_playerInput.DashInput)
		{
			m_rigid.AddForce(m_moveDir * dashPower, ForceMode.Impulse);
			//Debug.Log("Dash Used :" + moveDir);
			// Or you can do different way for example moving exactly 2 meters in some direction by Translate or lerping
		}
	}

	void PlayerJump()
	{

		if(m_isGrounded)
		{
			m_lastMoveDir = m_currentMoveDir;

			if(m_playerInput.JumpInput)
			{
				m_isGrounded = false;
				m_rigid.AddForce((Vector3.up) * jumpPower, ForceMode.Impulse);
				//m_rigid.velocity += Vector3.up * jumpPower;
			}
		}
		else
		{
			if(!m_playerInput.HoldingJumpInput)
			{
				m_rigid.velocity += Physics.gravity * lowFallMultiplier * Time.deltaTime;
			}
			else
			{
				m_rigid.velocity +=  Physics.gravity * gravityMultiplier * Time.deltaTime;
			}

			/*

			if(m_playerInput.NoInput())
			{
				m_rigid.AddForce(m_lastMoveDir * 40f, ForceMode.Acceleration);
			}
			 */
		}
		
	}

	void CalculateMovePosition()
	{
		Vector3 vDir = m_playerInput.V * m_CameraController.transform.forward;
		Vector3 hDir = m_playerInput.H * m_CameraController.transform.right;

		m_currentMoveDir = (vDir + hDir).normalized;

		float moveMagnitude = m_currentMoveDir.magnitude;

		Debug.Log(moveMagnitude);

		m_moveDir = Vector3.Lerp(m_moveDir, m_currentMoveDir, Time.deltaTime * turnSpeed);
				
		if(m_playerInput.NoInput())
		{
			m_smoothFactor = 0f;
		}
		else
		{
			m_smoothFactor = Mathf.SmoothDamp(m_smoothFactor, moveMagnitude, ref m_smoothVelocityRef, smoothTime );
		}

		m_moveVector = m_moveDir * moveSpeed * m_smoothFactor * Time.deltaTime ;

		if(m_moveVector != Vector3.zero)
		{
			m_lastMoveDir = m_moveVector;
			//Debug.Log(m_lastMoveDir);
		}
	}

}
