using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public static EnemyManager instance;
    List<Enemy> m_enemies = new List<Enemy>();
	int m_enemyCount;

    public List<Enemy> Enemies { get { return m_enemies; } set { m_enemies = value; } }

    public int EnemyCount { get { return m_enemyCount; } set { m_enemyCount = value; } }

    void Awake()
    {
        Singleton();
    }

	void Start()
	{
		//SubscribeToEnemies();
	}

	void SubscribeToEnemies()
	{
		foreach(Enemy enemy in m_enemies)
		{
			enemy.enemyHealth.OnEnemyDeath += DecreaseEnemyCount;
		}
	}

	public void DecreaseEnemyCount(EnemyHealth enemy)
	{
		m_enemyCount--;
		enemy.OnEnemyDeath -= DecreaseEnemyCount;
	}

    private void Singleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

}
