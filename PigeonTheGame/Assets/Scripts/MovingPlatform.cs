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
	public AnimationCurve moveCurve = AnimationCurve.Linear(0f,0f,1f,1f);

	public bool falling;
	public bool alreadyFall;

	public float fallingDelay = 1f;

	public WaypointNetwork waypoints;

	public float moveDuration;

	public float waitDuration = 0f;

	//Vector3 m_startPos;
	Vector3 m_targetWaypoint;

	Vector3 m_currentWaypoint;

	int m_currentWaypointIndex = 0;

	Rigidbody m_rigid;

    // Use this for initialization
    void Start()
    {
		m_rigid = GetComponent<Rigidbody>();
		waypoints.waypointsArray[0].position = transform.position;
		m_currentWaypoint = waypoints.waypointsArray[0].position;
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
				if(m_currentWaypointIndex < waypoints.waypointsArray.Length - 1 )
				{
					m_currentWaypointIndex++;

					if(m_currentWaypointIndex == waypoints.waypointsArray.Length - 1)
					{
						Array.Reverse(waypoints.waypointsArray);
						m_currentWaypointIndex = 0;
					}
				}
			}
			break;

			case MoveType.Loop:
			{
				m_currentWaypointIndex = (m_currentWaypointIndex + 1) % waypoints.waypointsArray.Length;
			}
			break;
		}

		m_targetWaypoint = waypoints.waypointsArray[m_currentWaypointIndex].position;

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
			if(alreadyFall)
				yield break;

			percent += Time.deltaTime * moveSpeed;
			transform.position = Vector3.Lerp(m_currentWaypoint, m_targetWaypoint, moveCurve.Evaluate(percent));

			yield return null;
		}

		if(waitDuration > 0)
			yield return new WaitForSeconds(waitDuration);

		//m_currentWaypoint = m_targetWaypoint;
		MoveToNextWaypoint();
	}

	void OnCollisionEnter(Collision other)
	{
		if(!alreadyFall)
		{
			if(other.gameObject.CompareTag("Player"))
				other.gameObject.transform.parent = transform;

			if(falling)
				StartCoroutine(FallPlatformAfterDelay(other.gameObject));
		}

	}

	void OnCollisionExit(Collision other)
	{
		if(other.gameObject.CompareTag("Player"))
			other.gameObject.transform.parent = null;
	}

	IEnumerator FallPlatformAfterDelay(GameObject go)
	{
		yield return new WaitForSeconds(fallingDelay);

		go.gameObject.transform.parent = null;

		m_rigid.useGravity = true;
		m_rigid.isKinematic = false;

		alreadyFall = true;
	}

}
