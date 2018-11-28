using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCheck : MonoBehaviour
{

	public RoomIndex roomIndex;
	public DoorMovement door;

	EnemySpawner m_enemySpawner;

	bool m_doorTriggered;

	// Use this for initialization
	void Start ()
	{
		m_enemySpawner = FindObjectOfType<EnemySpawner>();
		PlayerHealth.OnPlayerRespawn += ResetDoor;
	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider other)
	{
		if(other.gameObject.tag == "Player")
			if(m_enemySpawner.CheckIfAllEnemiesOnRoomAreDead(roomIndex) && !m_doorTriggered)
			{
				m_doorTriggered = true;
				door.InitDoor();
			}
	}

	void ResetDoor()
    {
        m_doorTriggered = false;

        if(door != null)
			door.ResetPos();
    }
}
