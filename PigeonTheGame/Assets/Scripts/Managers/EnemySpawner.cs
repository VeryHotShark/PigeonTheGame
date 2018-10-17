using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
	Melee,
	Range,
	Heavy,
	Stationary
}

public class EnemySpawner : MonoBehaviour
{

	public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
