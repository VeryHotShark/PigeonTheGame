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
    public float aimMoveSpeed;
    public float smoothTime;
    public float turnSpeed;

    [Space]
    [Header("Player Dash")]
    public float dashPower = 5f;
    public float dashDistance = 3f;
    public float dashTime = 0.2f;
    public LayerMask dashLayerMask;
    public AnimationCurve dashCurve;


    [Space]
    [Header("Player Jump")]
    public float jumpPower = 10f;
    public float gravityMultiplier = 2f;
    public float lowFallMultiplier = 3f;
    public LayerMask groundLayerMask;
    //public int wallLayerMask = 1 << 9;

    [Space]
    [Header("Player VFX")]

    public ParticleSystem smokeParticle;
    public TrailRenderer dashVFX;


    // PRIVATE VARIABLES

    Rigidbody m_rigid;
    PlayerInput m_playerInput;
    PlayerHealth m_playerHealth;
    CameraController m_CameraController;

    Vector3 m_moveVector;
    Vector3 m_moveDir;
    Vector3 m_currentMoveDir;
    Vector3 m_lastMoveDir;
    float m_smoothFactor;
    float m_smoothVelocityRef;

    float m_angle;

    bool m_isGrounded;
    bool m_dashing;
    bool m_dashedInAir;
    bool m_allowDash;

    // ANIMATOR

    Animator m_anim;

    // Convert our animator variablesw to Hashes so it more performent

    int m_directionXHash = Animator.StringToHash("X");
    int m_directionYHash = Animator.StringToHash("Y");
    int m_dashHash = Animator.StringToHash("Dash");
    int m_inAirHash = Animator.StringToHash("InAir");

    public Rigidbody Rigid
    { get { return m_rigid; } set { m_rigid = value; } }

    public Animator Anim
    { get { return m_anim; } set { m_anim = value; } }

    public Vector3 LastMoveVector { get { return m_lastMoveDir; } set { m_lastMoveDir = value; } }

    public PlayerHealth playerHealth { get { return m_playerHealth; }}

    void Awake()
    {
        GetComponents();

        PlayerHealth.OnPlayerDeath += DisableSmoke;
        PlayerHealth.OnPlayerRespawn += ReenableSmoke;

        aimMoveSpeed = moveSpeed / 2f; // speed is slower 2 times when aiming TODO change to variable
        dashVFX.emitting = false;
    }

    void GetComponents()
    {
        m_playerHealth = GetComponent<PlayerHealth>();
        m_playerInput = GetComponent<PlayerInput>();
        m_rigid = GetComponent<Rigidbody>();
        m_CameraController = FindObjectOfType<CameraController>();
        m_anim = GetComponentInChildren<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!m_playerHealth.IsDead())
        {
            CheckIfGrounded();
            PlayerDash();
            PlayerJump();
            SmokeTrail();
        }
    }

    void DisableSmoke()
    {
        var particle = smokeParticle;
        particle.gameObject.SetActive(false);
    }

    void ReenableSmoke()
    {
        var particle = smokeParticle;
        particle.gameObject.SetActive(true);
    }

    void FixedUpdate()
    {
        CalculateMovePosition();

        if (m_playerInput.InputEnabled)
            m_rigid.MovePosition(m_rigid.position + (m_playerInput.NoInput() && !m_isGrounded ? m_lastMoveDir / 2f : m_moveVector)); // Move our player based on calculated earlier direction

        UpdateRotation();
    }

    void UpdateRotation()
    {
        Quaternion desiredRot = m_CameraController.transform.rotation; // our player desired rot is cam rot

        if (!snapToCamera) // smooth our rotation if bool is false
            desiredRot = Quaternion.Slerp(transform.rotation, m_CameraController.transform.rotation, smoothRotateSpeed * Time.fixedDeltaTime);

        m_rigid.MoveRotation(desiredRot); // actually rotate our player
    }

    void CheckIfGrounded()
    {
        // Cast ray downwards to check if we are on ground

        Ray ray = new Ray(transform.position - transform.forward, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 0.6f, groundLayerMask))
        {
            m_anim.SetBool(m_inAirHash, false);
            m_playerInput.InputEnabled = true;
            m_allowDash = true;
            m_isGrounded = true;
        }
        else
        {
            m_isGrounded = false;
            m_anim.SetBool(m_inAirHash, true);
        }
    }

    void PlayerDash()
    {
        // If player press shift we add force to player

        if (m_playerInput.DashInput)
        {

            if (m_allowDash)
            {
                m_anim.SetTrigger(m_dashHash);

                StartCoroutine(StopDashVFX());

                if (!m_isGrounded)
                {
                    m_dashedInAir = true;
                    m_allowDash = false;
                }

                if (!m_dashing)
                    StartCoroutine(Dash());
            }


            //m_rigid.AddForce(m_moveDir * dashPower, ForceMode.Impulse);
        }
    }

    IEnumerator Dash()
    {
        //m_playerInput.InputEnabled = true;
        m_dashing = true;

        float percent = 0f;
        float speed = 1f / dashTime;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + m_moveDir.normalized * dashDistance;

        // CHECK IF THERE SOME OBSTACLE ON OUR WAY
        //Vector3 rayDir = new Vector3(m_moveDir.x, startPos.y,m_moveDir.z).normalized;
        Ray ray = new Ray(startPos, m_moveDir.normalized);
        RaycastHit hit;

        bool hitSomething = Physics.Raycast(ray, out hit, dashDistance, dashLayerMask, QueryTriggerInteraction.Ignore);


        if (hitSomething)
        {
            endPos = hit.point + hit.normal * 1f;
            Debug.DrawLine(startPos, endPos, Color.red, 3f);
        }

        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPos, endPos, dashCurve.Evaluate(percent));

            yield return null;
        }

        m_dashing = false;
        //m_playerInput.InputEnabled = false;
    }

    IEnumerator StopDashVFX()
    {
        dashVFX.emitting = true;
        yield return new WaitForSeconds(dashTime);
        dashVFX.emitting = false;
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

        m_moveVector = m_moveDir * (m_playerInput.ZoomInput ? aimMoveSpeed : moveSpeed) * m_smoothFactor * Time.deltaTime; // here we calculate our final moveVector

        //Debug.Log(m_moveVector);

        if (m_moveVector != Vector3.zero) // if our move Vector is other than Vector3.zero which means it has a direction
        {
            m_lastMoveDir = m_moveVector; // we assign that direction to our lastMoveDir variable (we will use that variable to move our player in mid-air)
            //Debug.Log(m_lastMoveDir);
        }

        // Convert our m_moveDir to local space so our animations will play correctly in local space
        Vector3 localMoveDir = transform.InverseTransformVector(m_moveDir);

        // Set our float in animator based on our move DIr
        m_anim.SetFloat(m_directionXHash, localMoveDir.x);
        m_anim.SetFloat(m_directionYHash, localMoveDir.z);
    }

    void PlayerJump()
    {

        if (m_isGrounded) // if we are on ground
        {
            m_lastMoveDir = m_currentMoveDir; // our lastMoveDir = our currentDir

            if (m_playerInput.JumpInput) // if we pressed jump button
            {
                m_anim.SetBool(m_inAirHash, true);
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

    void SmokeTrail()
    {
        var emission = smokeParticle.emission;

        if (m_playerInput.NoInput()) // if there is no input
        {
            emission.rateOverTime = 0f; // we emite no smoke
        }
        else
            emission.rateOverTime = 50f; // else our emission is 50f
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
