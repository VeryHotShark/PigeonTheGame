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
    //public int wallLayerMask = 1 << 9;



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

    public Rigidbody Rigid
    {
        get
        {
            return m_rigid;
        }

        set
        {
            m_rigid = value;
        }
    }

    public Vector3 LastMoveVector
    {
        get
        {
            return m_lastMoveDir;
        }

        set
        {
            m_lastMoveDir = value;
        }
    }

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

        if (m_playerInput.InputEnabled)
            m_rigid.MovePosition(m_rigid.position + (m_playerInput.NoInput() && !m_isGrounded ? m_lastMoveDir / 2f : m_moveVector));

        UpdateRotation();
    }

    void UpdateRotation()
    {
        Quaternion desiredRot = m_CameraController.transform.rotation;

        if (!snapToCamera)
            desiredRot = Quaternion.Slerp(transform.rotation, m_CameraController.transform.rotation, smoothRotateSpeed * Time.fixedDeltaTime);

        m_rigid.MoveRotation(desiredRot);
    }

    void CheckIfGrounded()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 0.6f, groundLayerMask))
        {
            m_playerInput.InputEnabled = true;
            m_isGrounded = true;
        }
        else
        {
            m_isGrounded = false;
        }
    }

    void PlayerDash()
    {
        if (m_playerInput.DashInput)
        {
            m_rigid.AddForce(m_moveDir * dashPower, ForceMode.Impulse);
            //Debug.Log("Dash Used :" + moveDir);
            // Or you can do different way for example moving exactly 2 meters in some direction by Translate or lerping
        }
    }

    void CalculateMovePosition()
    {
		// Calculate our horizontal and vertical direction using current Camera rotation so our movement is always Camera rotation dependent;

        Vector3 vDir = m_playerInput.V * m_CameraController.transform.forward;
        Vector3 hDir = m_playerInput.H * m_CameraController.transform.right;

        m_currentMoveDir = (vDir + hDir).normalized; // Combine two direction that we calculate before

        float moveMagnitude = m_currentMoveDir.magnitude; // assign our current move direction magnitude to a variable

        //Debug.Log(moveMagnitude);

        m_moveDir = Vector3.Lerp(m_moveDir, m_currentMoveDir, Time.deltaTime * turnSpeed); // here we apply our own input smoothing so changing between direction won't be co noticeable

        if (m_playerInput.NoInput()) // if there is no input
        {
            m_smoothFactor = 0f; // we set our smoot factor to 0 so our player will stop immediately
        }
        else
        {
            m_smoothFactor = Mathf.SmoothDamp(m_smoothFactor, moveMagnitude, ref m_smoothVelocityRef, smoothTime); // we calculate smoothFactor based on moveMagnitude
        }

        m_moveVector = m_moveDir * moveSpeed * m_smoothFactor * Time.deltaTime; // here we calculate our final moveVector

        //Debug.Log(m_moveVector);

        if (m_moveVector != Vector3.zero) // if our move Vector is other than Vector3.zero which means it has a direction
        {
            m_lastMoveDir = m_moveVector; // we assign that direction to our lastMoveDir variable (we will use that variable to move our player in mid-air)
            //Debug.Log(m_lastMoveDir);
        }
    }

    void PlayerJump()
    {

        if (m_isGrounded) // if we are on ground
        {
            m_lastMoveDir = m_currentMoveDir; // our lastMoveDir = our currentDir

            if (m_playerInput.JumpInput) // if we pressed jump button
            {
                m_isGrounded = false; // we are not on ground anymore
                m_rigid.AddForce((Vector3.up) * jumpPower, ForceMode.Impulse); // we add upwards force to make our player jump
            }
        }
        else // if we are in mid-air
        {
            if (!m_playerInput.HoldingJumpInput) // if we are not holding jump button
            {
                m_rigid.velocity += Physics.gravity * lowFallMultiplier * Time.deltaTime; // we apply additional gravity to make it fall faster
            }
            else
            {
                m_rigid.velocity += Physics.gravity * gravityMultiplier * Time.deltaTime; // else if we are holding jump button while in air we apply smaller gravity so we can jump higher
            }
        }

    }



    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall") && !m_isGrounded) // if we hit wall and we are not grounded
        {
            Debug.Log("HitEnter");
            m_playerInput.InputEnabled = false; // we disabel player input so we won't push into the wall
        }
    }

    /*
		void OnCollisionStay(Collision other)
		{
			if(other.gameObject.layer == LayerMask.NameToLayer("Wall") && !m_isGrounded)
			{
				Debug.Log("HitStay");
				m_rigid.velocity += Physics.gravity * lowFallMultiplier * 2f * Time.deltaTime;
			}
		}
	*/


    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Debug.Log("HitExit");
            m_playerInput.InputEnabled = true; // we re-enable input when we are not touching wall anymore
        }
    }

}
