using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

	public static GameManager instance;

	EnemyManager m_enemyManager;
	PlayerHealth m_playerHealth;

	public bool GameIsOver = false;

	public event System.Action OnGameOver;

	Transform m_boss;

    public Transform Boss { get { return m_boss; } set { m_boss = value; } }

    void Awake()
	{
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(gameObject);

		m_enemyManager = FindObjectOfType<EnemyManager>();
		m_playerHealth = FindObjectOfType<PlayerHealth>();
		PlayerHealth.OnPlayerBigDeath += RestartLevel;
	}

	void RestartLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void LoadMenu()
	{
		MainMenuManager.instance.GameOver = true;
		SceneManager.LoadScene(0);
	}

	public void InvokeEvent()
	{
		 if(OnGameOver != null)
            OnGameOver();
	}
}
