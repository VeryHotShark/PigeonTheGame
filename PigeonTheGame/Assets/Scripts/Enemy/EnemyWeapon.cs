using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
	public int damage;

    public Transform spawnPoint;

    public Projectile projectile;
    public float force;

    // Update is called once per frame
    public void ShootProjectile()
    {
        Projectile obj = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation) as Projectile;
        obj.OnProjectileSpawn(transform.forward, force, damage);
    }
}
