using UnityEngine;
using System.Collections;

public class BasicMover : MonoBehaviour {

	public enum SpinRotation {RotateX,RotateY,RotateZ}; // 3 options to choose from our inspector
	public SpinRotation spinAxis; // a variable of type SpinRotation that we have just created from above
	public enum MotionTranslate {LeftRight,UpDown,ForwardBackward}; //public enum so we can choose which direction our cube will move
	public MotionTranslate	direction; //then we can use this variable to set our Motion Translate for ex. direction = MotionTranslate.LeftRight

	public float spinSpeed = 180.0f;
	public float motionMagnitude = 0.1f;

	public bool doSpin = true; 
	public bool alwaysOnScreen;
	public bool doMotion = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ( )
	{
		if ( doSpin ) // if it is true
		// rotate around the up axis of the gameObject
		{
			switch ( spinAxis ) //depends on spinAxis value we selected we rotate on that axis that we chose
			{
				case SpinRotation.RotateY: //if we chode rotate on Y axis we rotate on Y axis
					gameObject.transform.Rotate ( Vector3.up * spinSpeed * Time.deltaTime,Space.World ) ; //relative to space world so it will rotate globally 
					break ;
				case SpinRotation.RotateX:
					gameObject.transform.Rotate ( Vector3.right * spinSpeed * Time.deltaTime,Space.World ) ; //if it would rotate locally it would mix translation and rotation
					break ;
				case SpinRotation.RotateZ:
					gameObject.transform.Rotate ( Vector3.forward * spinSpeed * Time.deltaTime ,Space.World) ; //and as a result we would get a fucked up moving cube
					break ;
			}
		}

		if ( doMotion ) //if bool is true
		// move up and down over time
		{
			switch(direction)//this same we move in direction that we chose from inspector
			{
				case MotionTranslate.UpDown:
					gameObject.transform.Translate ( Vector3.up * Mathf.Cos ( Time.timeSinceLevelLoad ) * motionMagnitude ,Space.World) ;	 //relative to space world so it will rotate globally 
					break;
				case MotionTranslate.LeftRight:
					gameObject.transform.Translate ( Vector3.right * Mathf.Cos ( Time.timeSinceLevelLoad ) * motionMagnitude,Space.World ) ;	//if it would rotate locally it would mix translation and rotation
					break;
				case MotionTranslate.ForwardBackward:
					gameObject.transform.Translate ( Vector3.forward * Mathf.Cos ( Time.timeSinceLevelLoad ) * motionMagnitude ,Space.World) ;	 //and as a result we would get a fucked up moving cube
					break;
			}
		}

		if(alwaysOnScreen)
		{
			
         Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
         transform.position = Camera.main.ViewportToWorldPoint(pos);
    
		}
	}
}
