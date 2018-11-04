using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	public EnemyType enemyType;
	public RoomIndex roomIndex;

	public float waitDelay;

    bool enemyAlive;
    Enemy m_enemy;

	public Enemy MyEnemy { get {return m_enemy;} set {m_enemy = value;}}
    public bool EnemyAlive { get { return enemyAlive; } set { enemyAlive = value; } }
}
