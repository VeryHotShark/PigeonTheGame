﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

	public bool inputEnabled = true;

	float m_v;
    public float V { get { return m_v; } set { m_v = value; } }

    float m_h;
    public float H { get { return m_h; } set { m_h = value; } }

	float m_mouseV;
    public float MouseV { get { return m_mouseV; } set { m_mouseV = value; } }

    float m_mouseH;
    public float MouseH { get { return m_mouseH; } set { m_mouseH = value; } }

    bool m_shootInput;
    public bool ShootInput { get { return m_shootInput; } set { m_shootInput = value; } }

	bool m_dashInput;
    public bool DashInput { get { return m_dashInput; } set { m_dashInput = value; } }

	bool m_jumpInput;
    public bool JumpInput { get { return m_jumpInput; } set { m_jumpInput = value; } }

	bool m_holdingJumpInput;
    public bool HoldingJumpInput { get { return m_holdingJumpInput; } set { m_holdingJumpInput = value; } }



    // Update is called once per frame
    void Update()
    {
		if(inputEnabled)
		{

			m_mouseH = Input.GetAxisRaw("Mouse X");
			m_mouseV = Input.GetAxisRaw("Mouse Y");

			m_h = Input.GetAxisRaw("Horizontal");
			m_v = Input.GetAxisRaw("Vertical");

			m_shootInput = Input.GetMouseButton(0);
			m_jumpInput = Input.GetKeyDown(KeyCode.Space);
			m_dashInput = Input.GetKeyDown(KeyCode.LeftShift);

			m_holdingJumpInput = Input.GetKey(KeyCode.Space);


		/*
			if(Input.GetKeyDown(KeyCode.Space))
				m_jumpInput = true;
			else if(Input.GetKeyUp(KeyCode.Space))
				m_jumpInput = false;
		 */
		
		/*
			Debug.Log("Horizontal: " + m_h);
			Debug.Log("Vertical: " + m_v);
			Debug.Log("Mouse Horizontal: " + m_mouseH);
			Debug.Log("Mouse Vertical: " + m_mouseV);

			Debug.Log("Dash Input: " + m_dashInput);
			Debug.Log("Jump Input: " + m_jumpInput);
			Debug.Log("Shoot Input: " + m_shootInput);
		 */
		}
    }

	public bool NoInput()
	{
		if(m_h == 0 && m_v == 0)
			return true;

		return false;
	}

	public Vector3 PlayerDirection()
	{
		return new Vector3(m_h,0f,m_v).normalized;
	}

}
