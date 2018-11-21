using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorTrap : MonoBehaviour
{



	public Vector2 minMaxRotation;
	public float rotationDuration;
	public AnimationCurve rotateAnimCurve;


	// Use this for initialization
	void Start ()
	{
		StartCoroutine(RotateRoutine());
	}
	
	IEnumerator RotateRoutine()
    {
        float percent = 0f;
        float moveSpeed = 1f / rotationDuration;

        while (percent < 1f)
        {
            percent += Time.deltaTime * moveSpeed;
			var eulerAnglesRot = transform.rotation.eulerAngles;
            eulerAnglesRot.z = Mathf.Lerp(minMaxRotation.x,minMaxRotation.y, rotateAnimCurve.Evaluate(percent));
			transform.rotation = Quaternion.Euler(eulerAnglesRot);

            yield return null;
        }

		StartCoroutine(RotateRoutine());
    }

}
