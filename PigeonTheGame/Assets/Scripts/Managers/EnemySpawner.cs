using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EnemyType
{
    Melee,
    Range,
    Heavy,
    Stationary,
    Boss
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
    public Dictionary<EnemyType, Queue<Enemy>> poolDictionary = new Dictionary<EnemyType, Queue<Enemy>>();
    GameObject parentTransform;

    List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    public static event System.Action OnAllEnemyDeadInRoom;

    void Start()
    {
        PlayerHealth.OnPlayerRespawn += RespawnDeadEnemies;
        RoomTrigger.OnPlayerEnterRoom += SpawnAdditionalEnemies;
        GameManager.instance.OnGameOver += Unsubscribe;

        spawnPoints = FindObjectsOfType<SpawnPoint>().ToList();
        parentTransform = CreateParent();

        foreach (EnemyPool enemyPool in enemyPools)
        {
            CreatePool(enemyPool.enemy, enemyPool.size, enemyPool.enemyType);
        }

        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (!spawnPoint.additionalSpawn)
                ReuseObject(spawnPoint.enemyType, spawnPoint.transform.position, spawnPoint.transform.rotation, spawnPoint);
        }
    }

    void Unsubscribe()
    {
        PlayerHealth.OnPlayerRespawn -= RespawnDeadEnemies;
        RoomTrigger.OnPlayerEnterRoom -= SpawnAdditionalEnemies;
        GameManager.instance.OnGameOver -= Unsubscribe;
    }

    void CreatePool(Enemy enemy, int poolSize, EnemyType enemyType)
    {
        if (!poolDictionary.ContainsKey(enemyType))
        {

            //GameObject parentTransform = CreateParent(enemy.roomIndex);

            Queue<Enemy> objectPool = new Queue<Enemy>();

            for (int i = 0; i < poolSize; i++)
            {
                Enemy enemyObj = Instantiate(enemy) as Enemy;
                enemyObj.gameObject.transform.parent = parentTransform.transform;
                enemyObj.GetComponents();
                enemyObj.enemyHealth.GetComponents();
                enemyObj.gameObject.SetActive(false);
                objectPool.Enqueue(enemyObj);
            }

            poolDictionary.Add(enemyType, objectPool);
        }
    }

    void RespawnDeadEnemies()
    {
        if(RoomManager.instance.PlayerInCorridor)
            return;

        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.roomIndex == RoomManager.instance.PlayerCurrentRoom)
            {
                if (!spawnPoint.additionalSpawn)
                {
                    if (!spawnPoint.EnemyAlive)
                    {
                        //spawnPoint.MyEnemy
                        ReuseObject(spawnPoint.enemyType, spawnPoint.transform.position, spawnPoint.transform.rotation, spawnPoint);
                    }
                    else
                    {
                        spawnPoint.MyEnemy.ResetAliveVariables();
                        //spawnPoint.MyEnemy.Init();
                    }
                }
                else
                {
                    if (spawnPoint.EnemyAlive)
                    {
                        //spawnPoint.MyEnemy.EnemyDied(spawnPoint.MyEnemy.enemyHealth);
                        spawnPoint.MyEnemy.ResetAdditionalSpawnValues();
                    }

                }

            }
        }
    }

    GameObject CreateParent(/*RoomIndex roomIndex*/)
    {
        //string parentName = roomIndex.ToString();
        return new GameObject("Enemies");
    }

    void ReuseObject(EnemyType enemyType, Vector3 position, Quaternion rotation, SpawnPoint spawnPoint)
    {
        if (poolDictionary.ContainsKey(enemyType))
        {
            Enemy objToReuse = poolDictionary[enemyType].Dequeue();
            
            objToReuse.OnEnemyDie += CheckIfAllDead;
            objToReuse.gameObject.SetActive(true);

            objToReuse.spawnPoint = spawnPoint;
            objToReuse.spawnPoint.EnemyAlive = true;
            objToReuse.spawnPoint.MyEnemy = objToReuse;

            objToReuse.roomIndex = spawnPoint.roomIndex;
            objToReuse.waitTimeWhenEnter = spawnPoint.waitDelay;

            objToReuse.enemyHealth.RagdollToggle(false);
            objToReuse.enemyHealth.ResetRagdollTransform();

            objToReuse.transform.position = spawnPoint.transform.position;
            objToReuse.transform.rotation = spawnPoint.transform.rotation;

            objToReuse.ResetVariables();
            //objToReuse.Init();

            poolDictionary[enemyType].Enqueue(objToReuse);
        }
    }

    void SpawnAdditionalEnemies(RoomIndex index)
    {
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.additionalSpawn && spawnPoint.roomIndex == index && RoomManager.instance.PlayerInRoom)
                StartCoroutine(SpawnAdditionalEnemiesRoutine(spawnPoint));
            //ReuseObject(spawnPoint.enemyType, spawnPoint.transform.position,spawnPoint.transform.rotation, spawnPoint);
        }
    }

    IEnumerator SpawnAdditionalEnemiesRoutine(SpawnPoint sp)
    {
        yield return new WaitForSeconds(sp.spawnDelay);

       if(RoomManager.instance.PlayerInCorridor)
         yield break;

        AudioManager.instance.PlayClipAt("Spawn", sp.transform.position);
        GameObject vfx = VFXPooler.instance.ReuseObject(VFXType.AppearSmoke,sp.transform.position,Quaternion.identity);

        yield return new WaitForSeconds(0.5f);

        ReuseObject(sp.enemyType, sp.transform.position, sp.transform.rotation, sp);
    }

    public bool CheckIfAllEnemiesOnRoomAreDead(RoomIndex index)
    {
        foreach(SpawnPoint spawnPoint in spawnPoints)
        {
            if(spawnPoint.roomIndex == index)
            {
                if(spawnPoint.EnemyAlive == false)
                    continue;
                else
                    return false;
            } 
        }
        
        return true;
    }

    public void CheckIfAllDead(RoomIndex index)
    {
        foreach(SpawnPoint spawnPoint in spawnPoints)
        {
            if(spawnPoint.roomIndex == index)
            {
                if(spawnPoint.EnemyAlive == false)
                    continue;
                else
                    return;
            } 
        }

        if(OnAllEnemyDeadInRoom != null)
            OnAllEnemyDeadInRoom();
    }

}
