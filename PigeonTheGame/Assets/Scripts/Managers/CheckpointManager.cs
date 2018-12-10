using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheckpointManager : MonoBehaviour
{

	public static CheckpointManager instance ;

	public List<Vector3> checkpoints = new List<Vector3>();
	public RoomTrigger m_currentCheckpoint;

    // Use this for initialization
    void Awake()
    {
		// Singleton
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(gameObject);

		List<RoomTrigger> checkpointsDoors = FindObjectsOfType<RoomTrigger>().ToList();

		foreach(RoomTrigger door in checkpointsDoors)
		{
			checkpoints.Add(door.transform.position);
		}

    }

}
