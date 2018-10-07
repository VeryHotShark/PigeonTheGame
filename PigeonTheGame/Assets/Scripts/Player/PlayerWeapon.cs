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

	public GameObject playerWeapon;

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
	}
	
	// Update is called once per frame
	void Update ()
	{

		RecoilWeapon();

		if(returning == false)
			LookAtCameraDir();

		if(m_playerInput.ShootInput)	
		{
			SpawnVFX();

			// RECOIL
			
			playerWeapon.transform.localPosition -= transform.InverseTransformDirection(playerWeapon.transform.forward) * kickPower;
			rotationWhenShot += rotationAmount;


			if(OnPlayerShoot != null)
				OnPlayerShoot();

			Ray ray = new Ray(m_camera.transform.position, m_camera.transform.forward);
			RaycastHit hit;
			bool hitSomething = Physics.Raycast(ray, out hit,100f, layerMask, QueryTriggerInteraction.Collide);

			if(hitSomething)
			{
				Vector3 shootDir = (hit.point - m_middleSpawnPoint.position).normalized;

				foreach(Transform spawnPoint in spawnPoints)
				{
					if(spawnPoint != spawnPoints[0])
					{
						Vector3 randomHitPoint = (hit.point + Random.insideUnitSphere * (m_playerInput.ZoomInput ? zoomSpreadPower : spreadPower )) + Vector3.up * 0.5f;

						shootDir = (randomHitPoint - spawnPoint.position).normalized;
					}

					Projectile obj = Instantiate(projectile,spawnPoint.position, spawnPoint.rotation) as Projectile;
					
					obj.OnProjectileSpawn(shootDir, force, damage, projectileLife);
				}
			}

			Debug.DrawLine(m_camera.transform.position, hit.point, Color.red, 2f);
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

			/*
			
				if(playerWeapon.transform.localPosition != weaponPosWhenShot || weaponPivot.localEulerAngles != rotationBeforeShot)
					returning = true;
				else
					returning = false;
			
			 */
			
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
	}

}
