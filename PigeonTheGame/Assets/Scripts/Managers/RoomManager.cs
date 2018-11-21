using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PropClass
{
    public Rigidbody rigid;

    [HideInInspector]
    public Vector3 pos;

    [HideInInspector]
    public Quaternion rot;
}

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

    public List<PropClass> propsList = new List<PropClass>();

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

    void SavePropsInitialValues()
    {
        foreach(PropClass prop in propsList)
        {
            prop.pos = prop.rigid.transform.position ;
            prop.rot = prop.rigid.transform.rotation;
        }
    }

    void ResetPropsValues()
    {
        foreach(PropClass prop in propsList)
        {
            prop.rigid.isKinematic = true;

            prop.rigid.transform.position = prop.pos;
            prop.rigid.transform.rotation = prop.rot;

            prop.rigid.isKinematic = false;
        }
    }

    void Start()
    {
        SavePropsInitialValues();
        PlayerHealth.OnPlayerDeath += ResetPlayerInRoom;
        PlayerHealth.OnPlayerRespawn += ResetPropsValues;
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
