using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

	[Header("Player Shoot")]
	[Space]

	public Transform desiredCamRot;
	public float returnSpeed;
	Vector3 initEulerRot;


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
	Quaternion initialRotation;

    // Use this for initialization
    void Start()
    {
		initialDuration = shakeDuration;
		initialPosition = transform.localPosition;
		initialRotation = transform.localRotation;
		initEulerRot = initialRotation.eulerAngles;

		PlayerWeapon.OnPlayerShoot += TiltCamera;
    }

    // Update is called once per frame
    void Update()
    {

		if(transform.localEulerAngles != initEulerRot)
			transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, initEulerRot, Time.deltaTime * returnSpeed);


		if(shakeDuration > 0f && isShaking)
		{
			switch(shakeType)
			{
				case ShakeType.Pos :
					transform.localPosition = initialPosition + Random.insideUnitSphere * power * Time.deltaTime;
					break;
				case ShakeType.Rot :
					transform.localRotation = Quaternion.LookRotation((Random.insideUnitSphere - transform.localPosition).normalized * power * Time.deltaTime);
					break;
				case ShakeType.PosRot :
				{
					transform.localPosition = initialPosition + Random.insideUnitSphere * power * Time.deltaTime;
					transform.localRotation = Quaternion.LookRotation((Random.insideUnitSphere - transform.localPosition).normalized * power * Time.deltaTime);
					break;
				}
			}

			shakeDuration -= Time.deltaTime;
		}
		else if(shakeDuration < 0f)
		{
			isShaking = false;
			shakeDuration = initialDuration;
			transform.localPosition = initialPosition;
			transform.localRotation = initialRotation;
		}
    }

	void TiltCamera()
	{
		transform.localRotation = desiredCamRot.localRotation;
	}
}
