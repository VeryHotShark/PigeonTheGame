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

    public DoorMovement[] doors;

    bool doorTriggered;
    public bool healthReset;

    bool m_healthResetted;

    public bool HealthResetted { get { return m_healthResetted; } }

    public static event System.Action OnPlayerEnterRoom;

    bool m_roomEntered;

    void Start()
    {
        if(doors != null)
            PlayerHealth.OnPlayerRespawn += ResetDoor;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !m_roomEntered)
        {
            switch (type)
            {
                case TriggerType.RoomEnter:
                    {
                        RoomManager.instance.PlayerInRoom = true;
                        RoomManager.instance.PlayerInCorridor = false;
                        RoomManager.instance.PlayerCurrentRoom = roomIndex;
                        m_roomEntered = true;

                        if(OnPlayerEnterRoom != null)
                            OnPlayerEnterRoom();
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
                        m_roomEntered = true;

                        if(OnPlayerEnterRoom != null)
                            OnPlayerEnterRoom();
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

            if(healthReset && !m_healthResetted)
                m_healthResetted = true;

        }
    }

    void OnTriggerEnter(Collider other)
    {
          if (other.gameObject.CompareTag("Player"))
          {
              if(triggerDoor)
                    if(doors != null && !doorTriggered)
                    {
                        doorTriggered = true;

                        foreach(DoorMovement door in doors)
                            door.InitDoor();
                    }
          }
    }

    void ResetDoor()
    {
        m_roomEntered = false;
        doorTriggered = false;
        m_healthResetted = false;

        if(doors != null)
            foreach(DoorMovement door in doors)
                door.ResetPos();
    }

}
