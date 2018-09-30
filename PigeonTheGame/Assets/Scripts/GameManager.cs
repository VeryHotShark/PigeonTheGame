using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

	EnemyManager m_enemyManager;
	PlayerHealth m_playerHealth;

	void Awake()
	{
		m_enemyManager = FindObjectOfType<EnemyManager>();
		m_playerHealth = FindObjectOfType<PlayerHealth>();
	}

    // Update is called once per frame
    void Update()
    {
		if(m_enemyManager.EnemyCount <= 0 || m_playerHealth.IsDead())
			if(Input.GetKeyDown(KeyCode.R))
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
