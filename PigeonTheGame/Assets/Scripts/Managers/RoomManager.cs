using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum RoomIndex
{
    First,
    Second,
    Third,
    Fourth,
    Fifth,
    FirstCorridor,
    SecondCorridor,
    ThirdCorridor,
    FourthCorridor
}


public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    RoomIndex m_playerCurrentRoom;
    bool m_playerInRoom = false;
    bool m_playerInCorridor = true;

    public bool PlayerInRoom { get { return m_playerInRoom; } set { m_playerInRoom = value; } }
    public bool PlayerInCorridor { get { return m_playerInCorridor; } set { m_playerInCorridor = value; } }
    public RoomIndex PlayerCurrentRoom { get { return m_playerCurrentRoom; } set { m_playerCurrentRoom = value; } }



    // Use this for initialization
    void Awake()
    {
        Singleton();
    }

    void Start()
    {
        PlayerHealth.OnPlayerRespawn += ResetPlayerInRoom;
    }

    private void Singleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void ResetPlayerInRoom()
    {
        m_playerInRoom = false;
        m_playerInCorridor = true;
    }
}
