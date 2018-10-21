using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{

	public GameObject muzzleflashVFX;
	ParticleSystem muzzlePS;

	public static event System.Action OnPlayerShoot;

	public LayerMask layerMask;

	public int damage = 1;
	public float timeBetweenShot = 0.5f;

	public GameObject playerWeapon;
	public Transform shellTransform;
	public GameObject shell;

	[Space]
	[Header("recoil")]

	public Transform weaponPivot;

	public float kickPower = 2f;
	public float rotationAmount = 30f;
	public float kickBackReturnTime = 0.1f;
	public float rotationReturnTime = 0.5f;

	float rotationVelocity;
	float rotationWhenShot;
	Vector3 rotationBeforeShot;

	Vector3 kickBackVelocity;
	Vector3 weaponPosWhenShot;

	bool returning;

	[Header("Shooting Variables")]
	[Space]

	public Transform[] spawnPoints;

	public Projectile projectile;
	public float projectileLife = 1f;

	public float spreadPower = 1f;
	public float zoomSpreadPower = 2f;
	public float force;

	PlayerInput m_playerInput;
	CameraController m_cameraController;
	Camera m_camera;

	Transform m_middleSpawnPoint;
	Animator m_anim;


	int m_shootHash = Animator.StringToHash("Shoot");

	float timer;

	// Use this for initialization
	void Start ()
	{
		GetComponents();

		m_middleSpawnPoint = spawnPoints[0];

		rotationBeforeShot = weaponPivot.localEulerAngles;
		weaponPosWhenShot = playerWeapon.transform.localPosition;
	}

	void GetComponents()
	{
		m_playerInput = GetComponent<PlayerInput>();
		m_cameraController = FindObjectOfType<CameraController>();
		m_camera = m_cameraController.GetComponentInChildren<Camera>();
		m_anim = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
	{

		RecoilWeapon();

		if(returning == false)
			LookAtCameraDir();

		// IF PLAYER PRESS SHOOT INPUT

		timer -= Time.deltaTime;

		if(m_playerInput.ShootInput)	
		{
			if(timer <= 0f)
			{
				m_anim.SetTrigger(m_shootHash);

				SpawnVFX();

				// RECOIL
				
				playerWeapon.transform.localPosition -= transform.InverseTransformDirection(playerWeapon.transform.forward) * kickPower;
				rotationWhenShot += rotationAmount;

				// CAST RAY TO CHECK IF WE HIT SOMETHING 
				Ray ray = new Ray(m_camera.transform.position, m_camera.transform.forward);
				RaycastHit hit;
				bool hitSomething = Physics.Raycast(ray, out hit,100f, layerMask, QueryTriggerInteraction.Collide);

				if(OnPlayerShoot != null)
					OnPlayerShoot();

				if(hitSomething) // If we did
				{
					Vector3 shootDir = (hit.point - m_middleSpawnPoint.position).normalized; // we calculate the shoot dir by substracting our spawn position from the point we hit

					foreach(Transform spawnPoint in spawnPoints) // foreach spawn point in our spawnpoints array
					{
						if(spawnPoint != spawnPoints[0]) // if its not middle spawnPoint
						{
							Vector3 randomHitPoint = (hit.point + Random.insideUnitSphere * (m_playerInput.ZoomInput ? zoomSpreadPower : spreadPower )) + Vector3.up * 0.5f; // We add to point we hit random point that is inside sphere with radius of 1 so it will not shoot directly in middle 

							shootDir = (randomHitPoint - spawnPoint.position).normalized; // and now it will be our shootDireciton
						}

						Projectile obj = Instantiate(projectile,spawnPoint.position, spawnPoint.rotation) as Projectile; // we spawn projectile
						
						obj.OnProjectileSpawn(shootDir, force, damage, projectileLife, transform.gameObject); // and we give it that direction ,force, damage etc
					}
				}

				Debug.DrawLine(m_camera.transform.position, hit.point, Color.red, 2f);

				timer = timeBetweenShot;
			}

		}

	}

	void RecoilWeapon()
	{
		if(playerWeapon != null)
		{
			if(playerWeapon.transform.localPosition != weaponPosWhenShot )
			{
				playerWeapon.transform.localPosition = Vector3.SmoothDamp(playerWeapon.transform.localPosition, weaponPosWhenShot, ref kickBackVelocity, kickBackReturnTime);
			}

			if(Vector3.Distance(playerWeapon.transform.localPosition, weaponPosWhenShot) > 0.005f)
				returning = true;
			else
				returning = false;

			rotationWhenShot = Mathf.SmoothDamp(rotationWhenShot, 0f, ref rotationVelocity, rotationReturnTime);
			weaponPivot.localEulerAngles = rotationBeforeShot - transform.InverseTransformDirection(weaponPivot.right) * rotationWhenShot;
			
		}
	}

	void LookAtCameraDir()
	{
		playerWeapon.transform.rotation = Quaternion.Lerp(playerWeapon.transform.rotation,Quaternion.LookRotation(m_camera.transform.forward),Time.deltaTime * 10f);
		//playerWeapon.transform.rotation = Quaternion.LookRotation(m_camera.transform.forward);
	}

	void SpawnVFX()
	{
		GameObject vfx = Instantiate(muzzleflashVFX,m_middleSpawnPoint.position,playerWeapon.transform.rotation) as GameObject;
		vfx.transform.parent = playerWeapon.transform;
		Destroy(vfx,2f);

		GameObject shellVFX = Instantiate(shell,shellTransform.position,shellTransform.rotation) as GameObject;
		//vfx.transform.parent = playerWeapon.transform;
		Destroy(shellVFX,3f);
	}

}
