using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovement : MonoBehaviour
{
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
        endPos = startPos - transform.up * moveUnit;
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
        float speed = 1f /  moveTime;

        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            curvedPercent = moveCurve.Evaluate(percent);
            transform.localPosition = Vector3.Lerp(startPos, endPos,  curvedPercent);
            yield return null;
        }

    }
}
