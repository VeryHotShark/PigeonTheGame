using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EnemyType
{
    Melee,
    Range,
    Heavy,
    Stationary
}

[System.Serializable]
public class EnemyPool
{
    public Enemy enemy;
    public EnemyType enemyType;
    public int size;
}

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    
    public List<EnemyPool> enemyPools = new List<EnemyPool>();
    public Dictionary<EnemyType,Queue<Enemy>> poolDictionary = new Dictionary<EnemyType, Queue<Enemy>>();

    List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    void Start()
    {
        spawnPoints = FindObjectsOfType<SpawnPoint>().ToList();

        foreach(EnemyPool enemyPool in enemyPools)
        {
            CreatePool(enemyPool.enemy,enemyPool.size, enemyPool.enemyType);
        }

        foreach(SpawnPoint spawnPoint in spawnPoints)
        {
            ReuseObject(spawnPoint.enemyType, spawnPoint.transform.position,spawnPoint.transform.rotation);
        }
    }

    void CreatePool(Enemy enemy, int poolSize, EnemyType enemyType)
    {
        if(!poolDictionary.ContainsKey(enemyType))
        {
            Queue<Enemy> objectPool = new Queue<Enemy>();

            for(int i = 0 ; i < poolSize ; i++)
            {
                Enemy enemyObj = Instantiate(enemy) as Enemy;
                enemyObj.GetComponents();
                enemyObj.gameObject.SetActive(false);
                objectPool.Enqueue(enemyObj);
            }

            poolDictionary.Add(enemyType,objectPool);
        }
    }

    void ReuseObject(EnemyType enemyType, Vector3 position,Quaternion rotation)
    {
        if(poolDictionary.ContainsKey(enemyType))
        {
            Enemy objToReuse = poolDictionary[enemyType].Dequeue();

            objToReuse.transform.position = position;
            objToReuse.transform.rotation = rotation;

            objToReuse.gameObject.SetActive(true);
            objToReuse.Init();
            //objToReuse.ResetVariables();

            poolDictionary[enemyType].Enqueue(objToReuse);
        }
    }

}
