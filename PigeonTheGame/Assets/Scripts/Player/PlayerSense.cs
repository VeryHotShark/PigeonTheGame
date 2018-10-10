using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSense : MonoBehaviour
{

	public Transform sensePivot;
	public GameObject senseSprite;
	EnemyHealth m_nearestEnemy;

	Camera m_camera;

	float dstToNearestEnemy = 10000f;

	void Start ()
	{
		m_camera = Camera.main;
		senseSprite.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		foreach(EnemyHealth enemy in EnemyManager.instance.Enemies)
		{
			if(enemy.IsDead())
				continue;

			float dst = Vector3.Distance(transform.position, enemy.transform.position);

			if(dst < dstToNearestEnemy)
			{
				dstToNearestEnemy = dst;
				m_nearestEnemy = enemy;
			}
		}

		if(RoomManager.instance.PlayerInRoom && m_nearestEnemy != null)
		{
			senseSprite.SetActive(true);
			//Quaternion lookRotation = Quaternion.LookRotation((m_nearestEnemy.position - sensePivot.position));
			//sensePivot.rotation = Quaternion.Euler(0, lookRotation.y, 0);
			sensePivot.transform.LookAt(m_nearestEnemy.transform);
			senseSprite.transform.LookAt(m_camera.transform);
		}
		else if(m_nearestEnemy == null)
			senseSprite.SetActive(false);


	}
}
