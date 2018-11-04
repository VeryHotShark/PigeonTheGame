using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{

    public RoomIndex roomIndex;

    public enum TriggerType
    {
        RoomEnter,
        RoomExit,
        Checkpoint,
        RoomEnterCheckpoint,
        RoomExitCheckpoint
    }

    public TriggerType type;

    public bool triggerDoor;

    public DoorMovement door;

    bool doorTriggered;

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
                        RoomManager.instance.PlayerCurrentRoom = roomIndex;
                    }
                    break;

                case TriggerType.RoomExit:
                    {
                        RoomManager.instance.PlayerInCorridor = true;
                        RoomManager.instance.PlayerInRoom = false;
                        RoomManager.instance.PlayerCurrentRoom = roomIndex;
                    }
                    break;

                case TriggerType.Checkpoint:
                    {
                        CheckpointManager.instance.m_currentCheckpoint = null;
                        CheckpointManager.instance.m_currentCheckpoint = this;
                    }
                    break;

                case TriggerType.RoomEnterCheckpoint:
                    {
                        RoomManager.instance.PlayerInRoom = true;
                        RoomManager.instance.PlayerInCorridor = false;
                        RoomManager.instance.PlayerCurrentRoom = roomIndex;
                        CheckpointManager.instance.m_currentCheckpoint = null;
                        CheckpointManager.instance.m_currentCheckpoint = this;
                    }
                    break;

                case TriggerType.RoomExitCheckpoint:
                    {
                        RoomManager.instance.PlayerInCorridor = true;
                        RoomManager.instance.PlayerInRoom = false;
                        RoomManager.instance.PlayerCurrentRoom = roomIndex;
                        CheckpointManager.instance.m_currentCheckpoint = null;
                        CheckpointManager.instance.m_currentCheckpoint = this;
                    }
                    break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
          if (other.gameObject.CompareTag("Player"))
          {
              if(triggerDoor)
                    if(door != null && !doorTriggered)
                    {
                        doorTriggered = true;
                        door.InitDoor();
                    }
          }
    }

}
