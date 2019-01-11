using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public BulletType bulletType;
	public int damage;
    public float speed;
    public float projectileLife = 3f;
    public Projectile projectile;

    public Transform spawnPoint;


    // Update is called once per frame
    public void ShootProjectile(Vector3 playerPos) // public Function to instantiate our projectile and set it parameters
    {
        //Projectile obj = Instantiate(projectile, spawnPoint.position, Quaternion.identity) as Projectile;
        Projectile obj = BulletPooler.instance.ReuseObject(bulletType,spawnPoint.position,spawnPoint.rotation);

        // Spawn VFX
        if(bulletType != BulletType.OwlBullet)
        {
            VFXPooler.instance.ReuseObject(VFXType.MuzzleFlashEnemy, spawnPoint.position,transform.rotation);
        }

        playerPos = playerPos + Vector3.up * 0.1f;
        obj.OnProjectileSpawn((playerPos - spawnPoint.position).normalized, speed, damage, projectileLife, transform.gameObject);
    }
}
