using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{

	public int damage = 1;

	public Transform spawnPoint;

	public Projectile projectile;
	public float force;

	PlayerInput m_playerInput;
	CameraController m_cameraController;


	// Use this for initialization
	void Start ()
	{
		GetComponents();
	}

	void GetComponents()
	{
		m_playerInput = GetComponent<PlayerInput>();
		m_cameraController = FindObjectOfType<CameraController>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(m_playerInput.ShootInput)	
		{
			Projectile obj = Instantiate(projectile,spawnPoint.position, spawnPoint.rotation) as Projectile;
			obj.OnProjectileSpawn(m_cameraController.GetCamera.transform.forward, force, damage);
		}
	}
}
