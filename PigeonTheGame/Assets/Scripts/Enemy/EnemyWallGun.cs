using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWallGun : MonoBehaviour 
{

	public Vector2 minMaxZScale;

	public AnimationCurve zScaleCurve;

	public float scaleDuration;

	public float timeBtwShots;

	EnemyWeapon m_enemyWeapon;

	Vector3 m_shootDir;

	// Use this for initialization
	void Start ()
	{
		GetComponents();

		m_shootDir = m_enemyWeapon.spawnPoint.position + m_enemyWeapon.spawnPoint.forward;


		StartCoroutine(StartShootRoutine());
	}

	void GetComponents()
	{
		m_enemyWeapon = GetComponent<EnemyWeapon>();

	}

	IEnumerator StartShootRoutine()
	{
		float percent = 0f;
		float speed = 1f / scaleDuration;

		while(percent < 1f)
		{
			percent += Time.deltaTime * speed;

			var transformZScale = transform.localScale;
			transformZScale.z = Mathf.Lerp(minMaxZScale.x,minMaxZScale.y, zScaleCurve.Evaluate(percent));
			transform.localScale = transformZScale;

			yield return null;
		}

		m_enemyWeapon.ShootProjectile(m_shootDir);
		yield return new WaitForSeconds(timeBtwShots);

		StartCoroutine(StartShootRoutine());
		
	}
}
