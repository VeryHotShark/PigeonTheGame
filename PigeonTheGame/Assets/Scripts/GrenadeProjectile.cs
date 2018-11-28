using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : Projectile
{

    public GameObject explosionVFX;

    public LayerMask layerMask;
    public float explosionRange = 5f;

    public override IEnumerator SizeOverLifetime()
    {
        float percent = 0f;
        float speed = 1f / m_lifeTime;

        float desiredSize;

        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            desiredSize = lifeSizeCurve.Evaluate(percent);
            transform.localScale = Vector3.Lerp(m_startSize, Vector3.zero, desiredSize);

            if (m_trailRenderer)
                m_trailRenderer.startWidth = Mathf.Lerp(m_trailStartWidth, 0f, desiredSize);

            yield return null;
        }

    /*
        GameObject vfxInstance = Instantiate(explosionVFX, transform.position, Quaternion.identity);
        Destroy(vfxInstance, 2f);
     */

        GameObject vfx = VFXPooler.instance.ReuseObject(VFXType.Explosion,transform.position,Quaternion.identity);

        CheckIfCollided();

        //Destroy(gameObject);
    }

    public override void OnCollisionEnter(Collision other)
    {
        if(other.collider.gameObject.tag == "Player")
            CheckIfCollided();
    }

    public void CheckIfCollided()
    {
        AudioManager.instance.PlayClipAt("HeavyExplosion", transform.position);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange, layerMask);

        if (colliders.Length > 0)
        {
            PlayerHealth playerHealth = colliders[0].GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
                playerHealth.GetComponent<PlayerMovement>().Rigid.AddExplosionForce(40f, transform.position, explosionRange, 2f, ForceMode.Impulse);
            }

        }

         gameObject.SetActive(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
