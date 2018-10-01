using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    public static RoomManager instance;

    bool m_playerInRoom = false;
    bool m_playerInCorridor = true;

    public bool PlayerInRoom { get { return m_playerInRoom; } set { m_playerInRoom = value; } }
    public bool PlayerInCorridor { get { return m_playerInCorridor; } set { m_playerInCorridor = value; } }

    // Use this for initialization
    void Awake()
    {
        Singleton();
    }

    private void Singleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
}
