using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

	CoinManager m_coinManager;

	Quaternion startRot;

	public void Init(CoinManager manager)
	{
		startRot = transform.localRotation;
		m_coinManager = manager;
	}

	// Use this for initialization
	void OnTriggerEnter()
	{
		m_coinManager.RemoveFromList(this);

		AudioManager.instance.PlayClipAt("Coin_01", transform.position);

		GameObject vfx = VFXPooler.instance.ReuseObject(VFXType.CoinPickUp,transform.position, startRot);
		gameObject.SetActive(false);
	}
	
}
