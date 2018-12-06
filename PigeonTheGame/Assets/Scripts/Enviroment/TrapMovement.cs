using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapMovement : MonoBehaviour
{

	public AudioClip spikesIn;
	public AudioClip spikesOut;

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

	AudioSource m_audioSource;

	WaitForSeconds yieldTime;
	WaitForSeconds yieldReturnTime;
	WaitForSeconds yieldDestinationTime;

	void Start()
	{
		Init();
		StartCoroutine(StartYieldTime());
	}

    void Init()
    {
		startPos = transform.localPosition;
		endPos = startPos + transform.forward * moveUnit;

		yieldTime = new WaitForSeconds(startYieldTime);
		yieldReturnTime = new WaitForSeconds(returnWaitTime);
		yieldDestinationTime = new WaitForSeconds(destinationYieldTime);

		m_audioSource = GetComponent<AudioSource>();
    }

	IEnumerator StartYieldTime()
	{
		yield return yieldTime;
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
		
		if(m_audioSource != null)
		{
			m_audioSource.clip = isReturning ? spikesOut : spikesIn;
			m_audioSource.Play();
		}
		
		while(percent < 1f)
		{
			percent += Time.deltaTime * speed;
			curvedPercent = moveCurve.Evaluate(percent);
			transform.localPosition = Vector3.Lerp(start, end, isReturning ? percent : curvedPercent);


			yield return null;
		}

		yield return (isReturning ? yieldReturnTime : yieldDestinationTime);

		returning = !returning;

		StartCoroutine(MoveToDestination(returning));
    }
}
