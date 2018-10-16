using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSense : MonoBehaviour
{

	public Transform sensePivot;
	public GameObject senseSprite;
	Enemy m_nearestEnemy;

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
		// FOR EACH ENEMY IN OUR ENEMY LIST WE CHECK IF THE DISTANCE TO THAT ENEMY IS SMALLER THAN OUR PREVIOUS DST TO ENEMY

		foreach(Enemy enemy in EnemyManager.instance.Enemies)
		{
			if(enemy.enemyHealth.IsDead())
				continue;

			float dst = Vector3.Distance(transform.position, enemy.transform.position);

			// IF IT IS WE SET THAT TO BE OUR NEAREST ENEMY AND WE UPDATE OUR DST

			if(dst < dstToNearestEnemy)
			{
				dstToNearestEnemy = dst;
				m_nearestEnemy = enemy;
			}
		}

		if(RoomManager.instance.PlayerInRoom && m_nearestEnemy != null) // IF PLAYER IS IN ROOM and we have nearest enemy
		{
			// WE SET SPRITE TO TRUE AND MAKE IT LOOK AT ENEMY

			senseSprite.SetActive(true);
			//Quaternion lookRotation = Quaternion.LookRotation((m_nearestEnemy.position - sensePivot.position));
			//sensePivot.rotation = Quaternion.Euler(0, lookRotation.y, 0);
			sensePivot.transform.LookAt(m_nearestEnemy.transform);
			senseSprite.transform.LookAt(m_camera.transform);
		}
		else if(m_nearestEnemy == null) // IF WE DON'T HAVE ENEMY WE SET SPRITE ACTIVE TO FALSE
			senseSprite.SetActive(false);


	}
}
