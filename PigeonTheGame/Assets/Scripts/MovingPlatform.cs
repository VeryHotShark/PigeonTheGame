using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

	public enum MoveType
	{
		PingPong,
		Horizontal,
		Vertical
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
		if(m_currentWaypointIndex < waypoints.Length - 1 )
		{
			m_currentWaypoint = transform.position;
			m_currentWaypointIndex++;
		}
		else
		{
			Array.Reverse(waypoints);
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

}
