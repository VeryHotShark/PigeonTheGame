using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

	public enum ShakeType
	{
		Pos,
		Rot,
		PosRot
	}

	public static ShakeType shakeType = ShakeType.Pos;

	// Camera Shake with rotation

	public float shakeDuration;
	public float power;

	public static bool isShaking;


	float initialDuration;
	Vector3 initialPosition;

    // Use this for initialization
    void Start()
    {
		initialDuration = shakeDuration;
		initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
		if(shakeDuration > 0f && isShaking)
		{
			transform.localPosition = initialPosition + Random.insideUnitSphere * power * Time.deltaTime;
			shakeDuration -= Time.deltaTime;
		}
		else if(shakeDuration < 0f)
		{
			isShaking = false;
			shakeDuration = initialDuration;
			transform.localPosition = initialPosition;
		}
    }
}
