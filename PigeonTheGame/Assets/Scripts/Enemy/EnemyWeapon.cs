using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
	public int damage;
    public float speed;
    public float projectileLife = 3f;
    public Projectile projectile;

    public Transform spawnPoint;


    // Update is called once per frame
    public void ShootProjectile() // public Function to instantiate our projectile and set it parameters
    {
        Projectile obj = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation) as Projectile;
        obj.OnProjectileSpawn(transform.forward, speed, damage, projectileLife, transform.gameObject);
    }
}
