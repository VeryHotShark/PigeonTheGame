using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{

	public void Start()
	{
		base.Init();
	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.Return))
		{
			base.TakeDamage(1);
			Debug.Log("Damage");
		}
	}

}
