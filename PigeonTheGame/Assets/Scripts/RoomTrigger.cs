using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{

    public enum TriggerType
    {
        RoomEnter,
        RoomExit,
    }

    public TriggerType type;

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            switch (type)
            {
                case TriggerType.RoomEnter:
                    {
                        RoomManager.instance.PlayerInRoom = true;
                        RoomManager.instance.PlayerInCorridor = false;
                    }
                    break;

                case TriggerType.RoomExit:
                    {
                        RoomManager.instance.PlayerInCorridor = true;
                        RoomManager.instance.PlayerInRoom = false;
                    }
                    break;
            }

            Debug.Log(RoomManager.instance.PlayerInRoom);
        }
    }

    void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.CompareTag("Player"))
		{
			CheckpointManager.instance.m_currentCheckpoint = null;
			CheckpointManager.instance.m_currentCheckpoint = this;
		}

    }

}
