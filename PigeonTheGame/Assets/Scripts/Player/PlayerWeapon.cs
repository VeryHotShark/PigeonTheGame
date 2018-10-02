using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{

	public LayerMask layerMask;

	public int damage = 1;

	public Transform[] spawnPoints;

	public Projectile projectile;
	public float spreadPower = 1f;
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
		if(m_playerInput.ShootInput)	
		{
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
						Vector3 randomHitPoint = hit.point + Random.insideUnitSphere * spreadPower;

						shootDir = (randomHitPoint - spawnPoint.position).normalized;
					}

					Projectile obj = Instantiate(projectile,spawnPoint.position, spawnPoint.rotation) as Projectile;
					
					obj.OnProjectileSpawn(shootDir, force, damage);
				}
			}

			Debug.DrawLine(m_camera.transform.position, hit.point, Color.red, 2f);
		}

	}

	void OnDrawGizmos()
	{

	}
}
