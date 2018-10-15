using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClip : MonoBehaviour {

    public Transform Player;
    Transform CameraPivot;
    Vector3 CameraStartPos;

    private void Start()
    {
        CameraPivot = transform.parent;
        CameraStartPos = transform.localPosition;
    }
    private void LateUpdate()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Player.position - Vector3.up * 0.3f, transform.position - Player.position, out hitInfo))
        {
            transform.localPosition = CameraPivot.InverseTransformPoint(hitInfo.point + Vector3.up * 0.3f);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, CameraStartPos, Time.deltaTime);
        }



    }

}
