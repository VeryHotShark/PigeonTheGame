﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BulletType
{
    PlayerBullet,
    EnemyBullet,
    HeavyBullet,
    None
}

[System.Serializable]
public class BulletPool
{
    public BulletType bulletType;
    public Projectile projectile;
    public int size;
}

public class BulletPooler : MonoBehaviour
{

    public static BulletPooler instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public List<BulletPool> bulletsPools = new List<BulletPool>();
    public Dictionary<BulletType, Queue<Projectile>> poolDictionary = new Dictionary<BulletType, Queue<Projectile>>();

    // Use this for initialization
    void Start()
    {
        foreach (BulletPool bulletPool in bulletsPools)
        {
            CreatePool(bulletPool.projectile, bulletPool.size, bulletPool.bulletType);
        }
    }

	void CreatePool(Projectile projectile, int poolSize, BulletType bulletType)
    {
        if(!poolDictionary.ContainsKey(bulletType))
        {
            
            //GameObject parentTransform = CreateParent(enemy.roomIndex);

            Queue<Projectile> objectPool = new Queue<Projectile>();

            for(int i = 0 ; i < poolSize ; i++)
            {
                Projectile projectileObj = Instantiate(projectile) as Projectile;
                //projectileObj.gameObject.transform.parent = parentTransform.transform;

				projectileObj.GetComponents();
                projectileObj.gameObject.SetActive(false);
                objectPool.Enqueue(projectileObj);
            }

            poolDictionary.Add(bulletType,objectPool);
        }
    }

	public Projectile ReuseObject(BulletType bulletType, Vector3 position,Quaternion rotation)
    {
        if(poolDictionary.ContainsKey(bulletType))
        {
            Projectile objToReuse = poolDictionary[bulletType].Dequeue();

            objToReuse.gameObject.SetActive(true);
            objToReuse.ResetVariables();

            objToReuse.transform.position = position;
            objToReuse.transform.rotation = rotation;

            //objToReuse.Init();

            poolDictionary[bulletType].Enqueue(objToReuse);
            return objToReuse;
        }
        else
            return null;
    }
}
