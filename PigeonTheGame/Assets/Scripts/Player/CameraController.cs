﻿using System;
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

	public Transform targetToFollow;
	public Transform targetWhenDead;
	Transform targetWhenWin;
	public Transform yawTransform;
	public float sensitivity = 1f;
	public bool invertY = false;
	public Vector2 lookAngle;
	public float smoothRotSpeed;
	public float followDelay = 0.5f;
	public float followMultiplierWhenStationary = 2f;

	public Transform crosshair;

	[Header("Marks Variables")]
	public GameObject deathMark;

	public float deathMarkStartSize;
	public float deathMarkEndSize;
	public float deathMarkDuration;
	public AnimationCurve deathMarkCurve;

	[Space]
	public GameObject hitMark;

	public float hitMarkStartSize;
	public float hitMarkEndSize;
	public float hitMarkDuration;
	public AnimationCurve hitMarkCurve;


	[Header("Zoom Variables")]

	public Transform zoomTransform;
	public float zoomSpeed ;
	public bool changeFOV;
	public float zoomFOV = 30f;
	public float zoomCrosshairScale = 1f;
	public float shootCrosshairMultiplier = 1.2f;

	[Header("Dash Variables")]
	public float dashFOV = 70f;
	public float dashDuration;
	public AnimationCurve dashAnimCurve;

	bool m_duringFOVChange;


	float m_initFOV;
	Vector3 m_cameraStartPos;
	Vector3 m_crosshairInitScale;
	Vector3 m_zoomCrosshairScale;

	bool  m_zoomingFinish = true;

	PlayerInput m_playerInput;
	Camera m_camera;

	public Camera GetCamera { get { return m_camera;}}
    public bool ZoomingFinish { get { return m_zoomingFinish; } set { m_zoomingFinish = value; } }
    public bool DuringFOVChange { get { return m_duringFOVChange; } set { m_duringFOVChange = value; } }

    float m_yaw;
	float m_pitch;

	Vector3 smoothRefVelocity;

	Vector3 m_offset;

	Transform m_target;


	IEnumerator DeathRoutine;
	IEnumerator HitRoutine;
	IEnumerator DashFOVRoutine;

    // Use this for initialization
    void Start()
    {
		GetComponents();
		Init();

    }

	void Init()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		m_target = targetToFollow;
		transform.position = m_target.position;
		m_cameraStartPos = m_camera.transform.localPosition;

		PlayerWeapon.OnPlayerShoot += ZoomCrosshair; // we zoom our crosshair when playter shoot
		PlayerHealth.OnPlayerDeath += DisableCamWhenDead;
		PlayerHealth.OnPlayerRespawn += ResetCam;
		PlayerHealth.OnPlayerRespawn += ReenableCamWhenRespawn;
		PlayerMovement.OnPlayerDash += ChangeDashFOV;

		GameManager.instance.OnGameOver += CamWhenWin;
		GameManager.instance.OnGameOver += Unsubscribe;

		deathMark.SetActive(false);
		hitMark.SetActive(false);
		EnemyHealth.OnEnemyTakeDamage += ShowHitMark;
		EnemyHealth.OnAnyEnemyDeath += ShowDeathMark;

		m_initFOV = m_camera.fieldOfView;
		m_crosshairInitScale = crosshair.localScale;
		m_zoomCrosshairScale = Vector3.one * zoomCrosshairScale;

		// WE SUBSTRACT INIT CAMERA ROTATION FROM MIN-MAX ANGLE

		lookAngle.x -= Mathf.Abs(m_camera.transform.eulerAngles.x);
		lookAngle.y -= Mathf.Abs(m_camera.transform.eulerAngles.x);
	}

	void Unsubscribe()
	{
		GameManager.instance.OnGameOver -= CamWhenWin;
		GameManager.instance.OnGameOver -= Unsubscribe;

		PlayerWeapon.OnPlayerShoot -= ZoomCrosshair; // we zoom our crosshair when playter shoot
		PlayerHealth.OnPlayerDeath -= DisableCamWhenDead;
		PlayerHealth.OnPlayerRespawn -= ResetCam;
		PlayerHealth.OnPlayerRespawn -= ReenableCamWhenRespawn;
		PlayerMovement.OnPlayerDash -= ChangeDashFOV;

		EnemyHealth.OnEnemyTakeDamage -= ShowHitMark;
		EnemyHealth.OnAnyEnemyDeath -= ShowDeathMark;
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
		if(m_target == null) // IF THERE IS NO TARGET , RETURN
			return;

		ZoomCamera();
		ResizeCrosshair();

		m_offset = m_target.position - transform.position; // offset between player and our position
        Vector3 desiredPos = transform.position + m_offset; // desired position that we want to be

		if(cameraType == CameraType.SmoothFollow)
			transform.position = Vector3.SmoothDamp(transform.position, m_target.position, ref smoothRefVelocity, followDelay * (m_playerInput.NoInput() ? 1f : followMultiplierWhenStationary)); // smooth our camera
		else
			transform.position = m_target.position; // snap our camera

    }

	void DisableCamWhenDead()
	{
		m_playerInput.MouseEnabled = false;
		m_playerInput.InputEnabled = false;
		m_target = targetWhenDead;
	}

	void CamWhenWin()
	{
		
		targetWhenWin = GameManager.instance.Boss.transform;

		//m_playerInput.MouseEnabled = false;
		//m_playerInput.InputEnabled = false;
		
		//m_camera.transform.LookAt(targetWhenWin);
	}

	void ReenableCamWhenRespawn()
	{
		m_playerInput.MouseEnabled = true;
		m_playerInput.InputEnabled = true;
		m_target = targetToFollow;
	}

    // Update is called once per frame
    void UpdateCamRotation()
    {
		// Calculate our yaw and pitch variable based on mouse input

		m_yaw -= m_playerInput.MouseV * (invertY ? -1 : 1); // reverse our yaw depending on bool
		m_pitch += m_playerInput.MouseH;

		m_yaw = Mathf.Clamp(m_yaw,lookAngle.x, lookAngle.y); // we clamp yaw value so we can't rotate fully 360 around x axis

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

	void ZoomCamera()
	{
		if(m_playerInput.ZoomInput) // if player pressed zoom button
		{
			m_zoomingFinish = false; // we set bool zoomingFinish to false

			// Lerp our cam pos , cam fov and crosshait Scale to desired values

			m_camera.transform.localPosition = Vector3.Lerp(m_camera.transform.localPosition, zoomTransform.localPosition, zoomSpeed * Time.deltaTime);
			m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, zoomFOV, zoomSpeed * Time.deltaTime);
			crosshair.localScale = Vector3.Lerp(crosshair.localScale, m_zoomCrosshairScale, zoomSpeed * Time.deltaTime);
		}
		else // if player is not holding zoom button
		{
			if(Vector3.Distance(m_camera.transform.localPosition,m_cameraStartPos) > 0.01f && m_zoomingFinish == false ) // if our camera pos is different than its start pos we zoom out
			{
				// Lerp our cam pos , cam fov and crosshait Scale to initial values

				m_camera.transform.localPosition = Vector3.Lerp(m_camera.transform.localPosition, m_cameraStartPos, zoomSpeed * Time.deltaTime);
				m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, m_initFOV, zoomSpeed * Time.deltaTime);
				crosshair.localScale = Vector3.Lerp(crosshair.localScale, m_crosshairInitScale, zoomSpeed * Time.deltaTime);
			}
			else // if the distance between cam start pos and cam pos now is smaller than 0,01f we snap all those values to initail values
			{
				if(m_zoomingFinish == false) // we do it only when we are not finish zooming so the code below will execute only one time
				{
					m_zoomingFinish = true;

					if(m_zoomingFinish)
					{
						m_camera.transform.localPosition = m_cameraStartPos;
						m_camera.fieldOfView = m_initFOV;
						crosshair.localScale = m_crosshairInitScale;
					}
				}
			}
		}
	}

	void ResizeCrosshair() // if our crosshair size is not initial size we lerp it to initial value
	{
		if(crosshair.localScale != m_crosshairInitScale && m_zoomingFinish == true)
			crosshair.localScale = Vector3.Lerp(crosshair.localScale, m_crosshairInitScale, zoomSpeed * Time.deltaTime);
	}	

	void ZoomCrosshair() // zoom crosshair when shoot
	{
		if(crosshair != null)
			crosshair.localScale = crosshair.localScale * shootCrosshairMultiplier;
		//Debug.Log(crosshair.localScale);
	}

	void ChangeDashFOV()
	{
		DashFOVRoutine = DashScreenRoutine();

		if(DashFOVRoutine != null)
			StartCoroutine(DashFOVRoutine);
	}

	IEnumerator DashScreenRoutine()
	{
		m_duringFOVChange = true;

		float percent = 0f;
		float speed = 1f / dashDuration;

		while(percent < 1f)
		{
			percent += Time.deltaTime * speed;
			m_camera.fieldOfView = Mathf.Lerp(m_initFOV,dashFOV, dashAnimCurve.Evaluate(percent));
			yield return null;
		}

		m_duringFOVChange = false;
	}

	void ResetCam()
	{
		m_yaw = 0f;
		m_pitch = CheckpointManager.instance.m_currentCheckpoint.transform.localEulerAngles.y;
		//transform.rotation = CheckpointManager.instance.m_currentCheckpoint.transform.localRotation;
	}

	void ShowDeathMark()
	{
		DeathRoutine = ShrinkDeathMarkRoutine();

		if(DeathRoutine != null)
			StartCoroutine(DeathRoutine);
	}

	void ShowHitMark()
	{
		HitRoutine = GrowHitMarkRoutine();

		if(HitRoutine != null && hitMark.activeSelf)
			StartCoroutine(HitRoutine);
	}
	
	IEnumerator ShrinkDeathMarkRoutine()
	{
		deathMark.SetActive(true);

		Vector3 startSize = Vector3.one * deathMarkStartSize;
		Vector3 endSize = Vector3.one * deathMarkEndSize;

		deathMark.transform.localScale = startSize;

		float percent = 0f;
		float speed = 1f / deathMarkDuration;

		while(percent < 1f)
		{
			percent += Time.deltaTime * speed;
			deathMark.transform.localScale = Vector3.Lerp(startSize, endSize, deathMarkCurve.Evaluate(percent)) ;
			yield return null;
		}

		deathMark.SetActive(false);

	}

	IEnumerator GrowHitMarkRoutine()
	{
		hitMark.SetActive(true);

		Vector3 startSize = Vector3.one * hitMarkStartSize;
		Vector3 endSize = Vector3.one * hitMarkEndSize;

		hitMark.transform.localScale = startSize;

		float percent = 0f;
		float speed = 1f / hitMarkDuration;

		while(percent < 1f)
		{
			percent += Time.deltaTime * speed;
			hitMark.transform.localScale = Vector3.Lerp(startSize, endSize, hitMarkCurve.Evaluate(percent)) ;
			yield return null;
		}

		hitMark.SetActive(false);

	}
}
