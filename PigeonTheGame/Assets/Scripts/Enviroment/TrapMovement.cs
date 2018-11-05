using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapMovement : MonoBehaviour
{

	public float moveUnit;
	public float moveTime;
	public AnimationCurve moveCurve;
	public float returnTime;
	public float startYieldTime;
	public float destinationYieldTime;
	public float returnWaitTime;

	Vector3 startPos;
	Vector3 endPos;

	bool returning = false;

	void Start()
	{
		Init();
		StartCoroutine(StartYieldTime());
	}

    void Init()
    {
		startPos = transform.localPosition;
		endPos = startPos + transform.forward * moveUnit;
    }

	IEnumerator StartYieldTime()
	{
		yield return new WaitForSeconds(startYieldTime);
		StartCoroutine(MoveToDestination(returning));
	}

    // Update is called once per frame
    IEnumerator MoveToDestination(bool isReturning)
    {
		float curvedPercent = 0f;
		float percent = 0f;
		float speed = 1f / (isReturning ? returnTime : moveTime);

		Vector3 start = isReturning ? endPos : startPos;
		Vector3 end = isReturning ? startPos : endPos;

		AudioManager.instance.PlayClipAt(isReturning ? "Spikes_Out" : "Spikes_In", transform.position);
		
		while(percent < 1f)
		{
			percent += Time.deltaTime * speed;
			curvedPercent = moveCurve.Evaluate(percent);
			transform.localPosition = Vector3.Lerp(start, end, isReturning ? percent : curvedPercent);


			yield return null;
		}

		yield return new WaitForSeconds(isReturning ? returnWaitTime : destinationYieldTime);

		returning = !returning;

		StartCoroutine(MoveToDestination(returning));
    }
}
