using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum RotateAxis
    {
        X,
        Y,
        Z
    }

    public enum MoveType
    {
        PingPong,
        Loop,
        Static,
        Rotate,
        LaunchPad
    }

	[Header("General Variables")]
	[Space]

    public MoveType moveType;
    public bool falling;
    public float fallingDelay = 1f;

	[Header("Moving Platform")]
	[Space]
    public AnimationCurve moveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);



    public WaypointNetwork waypoints;

    public float moveDuration;

    public float waitDuration = 0f;

	[Header("Rotate Platform")]
	[Space]

    public RotateAxis rotateAxis;

    public float rotateSpeed;

	[Header("Launch Platform")]
	[Space]

	public float launchForce;


    Vector3 m_startPos;
    Quaternion m_startRot;

    Vector3 m_targetWaypoint;

    Vector3 m_currentWaypoint;
    bool alreadyFall;

    int m_currentWaypointIndex = 0;

    Rigidbody m_rigid;


    // Use this for initialization
    void Start()
    {
        m_rigid = GetComponent<Rigidbody>();

        if (falling)
        {
            PlayerHealth.OnPlayerDeath += Reset;
            GameManager.instance.OnGameOver += Unsubscribe;
        }

        m_startPos = transform.position;
        m_startRot = transform.rotation;


        Init();

    }

    void Unsubscribe()
    {
        PlayerHealth.OnPlayerDeath -= Reset;
        GameManager.instance.OnGameOver -= Unsubscribe;
    }

    void Init()
    {


        if (waypoints != null)
        {
            waypoints.waypointsArray[0].position = transform.position;
            m_currentWaypoint = waypoints.waypointsArray[0].position;
        }

        if (moveType != MoveType.Static && waypoints != null)
            MoveToNextWaypoint();
    }

    void Update()
    {
        if (moveType == MoveType.Rotate)
        {
            switch (rotateAxis)
            {
                case RotateAxis.Y: //if we chode rotate on Y axis we rotate on Y axis
                    gameObject.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World); //relative to space world so it will rotate globally 
                    break;
                case RotateAxis.X:
                    gameObject.transform.Rotate(Vector3.right * rotateSpeed * Time.deltaTime, Space.World); //if it would rotate locally it would mix translation and rotation
                    break;
                case RotateAxis.Z:
                    gameObject.transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime, Space.World); //and as a result we would get a fucked up moving cube
                    break;
            }
        }
    }

    // Update is called once per frame
    void GetNextWaypoint()
    {
        m_currentWaypoint = transform.position;

        switch (moveType)
        {
            case MoveType.PingPong:
                {
                    if (m_currentWaypointIndex < waypoints.waypointsArray.Length - 1)
                    {
                        m_currentWaypointIndex++;

                        if (m_currentWaypointIndex == waypoints.waypointsArray.Length - 1)
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

        while (percent < 1f)
        {
            if (alreadyFall)
                yield break;

            percent += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(m_currentWaypoint, m_targetWaypoint, moveCurve.Evaluate(percent));

            yield return null;
        }

        if (waitDuration > 0)
            yield return new WaitForSeconds(waitDuration);

        //m_currentWaypoint = m_targetWaypoint;
        MoveToNextWaypoint();
    }

    void OnCollisionEnter(Collision other)
    {
        if (!alreadyFall && moveType != MoveType.LaunchPad)
        {
            if (other.gameObject.CompareTag("Player"))
                other.gameObject.transform.parent = transform;

            if (falling)
                StartCoroutine(FallPlatformAfterDelay(other.gameObject));
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && moveType == MoveType.LaunchPad) // if we hit wall and we are not grounded
        {
            Rigidbody rigid = other.gameObject.GetComponent<Rigidbody>();
            rigid.AddForce(transform.up * launchForce, ForceMode.Impulse);
        }

    }

    void OnCollisionExit(Collision other)
    {
        if (moveType != MoveType.LaunchPad)
            if (other.gameObject.CompareTag("Player"))
                other.gameObject.transform.parent = null;
    }

    IEnumerator FallPlatformAfterDelay(GameObject go)
    {
        AudioManager.instance.PlayClipAt("FallingPlatform", transform.position);

        yield return new WaitForSeconds(fallingDelay);

        go.gameObject.transform.parent = null;

        m_rigid.useGravity = true;
        m_rigid.isKinematic = false;

        alreadyFall = true;
    }

    void Reset()
    {
        transform.position = m_startPos;
        transform.rotation = m_startRot;

        if (m_rigid != null)
        {
            m_rigid.isKinematic = true;
            m_rigid.useGravity = false;
        }

        alreadyFall = false;

        Init();
    }

}
