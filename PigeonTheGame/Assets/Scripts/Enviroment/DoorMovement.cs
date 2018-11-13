using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovement : MonoBehaviour
{

    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public MoveDirection moveDirection;

    public float moveUnit;
    public float moveTime;
    public AnimationCurve moveCurve;
    public float startYieldTime;

    Vector3 startPos;
    Vector3 endPos;

    public void InitDoor()
    {
        Init();
        StartCoroutine(StartYieldTime());
    }

    void Init()
    {
        startPos = transform.localPosition;

        if (moveDirection == MoveDirection.Down)
            endPos = startPos - transform.up * moveUnit;
        else if (moveDirection == MoveDirection.Up)
            endPos = startPos + transform.up * moveUnit;
        else if (moveDirection == MoveDirection.Left)
            endPos = startPos - transform.right * moveUnit;
        else
            endPos = startPos + transform.right * moveUnit;
    }

    IEnumerator StartYieldTime()
    {
        yield return new WaitForSeconds(startYieldTime);
        StartCoroutine(MoveToDestination());
    }

    // Update is called once per frame
    IEnumerator MoveToDestination()
    {
        float curvedPercent = 0f;
        float percent = 0f;
        float speed = 1f / moveTime;

        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            curvedPercent = moveCurve.Evaluate(percent);
            transform.localPosition = Vector3.Lerp(startPos, endPos, curvedPercent);
            yield return null;
        }

    }

    public void ResetPos()
    {
        transform.localPosition = startPos;
    }
}
