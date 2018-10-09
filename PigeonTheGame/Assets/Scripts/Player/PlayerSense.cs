using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSense : MonoBehaviour
{

	public GameObject senseSprite;
	Transform nearestEnemy;

	float dstToNearestEnemy = 10000f;

	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		foreach(EnemyHealth enemy in EnemyManager.instance.Enemies)
		{
			float dst = Vector3.Distance(transform.position, enemy.transform.position);

			if(dst < dstToNearestEnemy)
				nearestEnemy = enemy.transform;
		}

		
	}
}
