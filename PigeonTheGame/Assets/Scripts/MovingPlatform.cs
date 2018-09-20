using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

	public enum MoveType
	{
		PingPong,
		Loop
	}

	public MoveType moveType;

	public Transform[] waypoints;

	public float moveDuration;

	//Vector3 m_startPos;
	Vector3 m_targetWaypoint;

	Vector3 m_currentWaypoint;

	int m_currentWaypointIndex = 0;

    // Use this for initialization
    void Start()
    {
		waypoints[0].position = transform.position;
		m_currentWaypoint = waypoints[0].position;
		MoveToNextWaypoint();
    }

    // Update is called once per frame
    void GetNextWaypoint()
    {
		m_currentWaypoint = transform.position;

		switch(moveType)
		{
			case MoveType.PingPong:
			{
				if(m_currentWaypointIndex < waypoints.Length - 1 )
				{
					m_currentWaypointIndex++;

					if(m_currentWaypointIndex == waypoints.Length - 1)
					{
						Array.Reverse(waypoints);
						m_currentWaypointIndex = 0;
					}
				}
			}
			break;

			case MoveType.Loop:
			{
				m_currentWaypointIndex = (m_currentWaypointIndex + 1) % waypoints.Length;
			}
			break;
		}

		m_targetWaypoint = waypoints[m_currentWaypointIndex].position;

    }

	void MoveToNextWaypoint()
	{
		GetNextWaypoint();
		StartCoroutine(MoveToNextWaypointRoutine());
	}

	IEnumerator MoveToNextWaypointRoutine()
	{
		float moveSpeed = 1f / moveDuration;
		float percent = 0f;

		while(percent < 1f)
		{
			percent += Time.deltaTime * moveSpeed;
			transform.position = Vector3.Lerp(m_currentWaypoint, m_targetWaypoint, percent );

			yield return null;
		}

		//m_currentWaypoint = m_targetWaypoint;
		MoveToNextWaypoint();
	}

	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.CompareTag("Player"))
			other.gameObject.transform.parent = transform;
	}

	void OnCollisionExit(Collision other)
	{
		if(other.gameObject.CompareTag("Player"))
			other.gameObject.transform.parent = null;
	}

}
