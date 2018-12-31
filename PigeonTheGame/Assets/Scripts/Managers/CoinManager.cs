using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class CoinManager : MonoBehaviour
{

	public TextMeshProUGUI coinText;
	public AnimationCurve scaleCurve;
	public float scaleDuration;
	public float desiredSize;
	

	Vector3 startSize;
	string x = " x";

	public int coinCount;

	public float spinSpeed;

	[Range(0f,0.8f)]
	public float moveRange;

	public float moveSpeed;

	public List<Coin> coinList = new List<Coin>();

	bool m_duringRoutine;

	// Use this for initialization
	void Awake ()
	{
		coinList = FindObjectsOfType<Coin>().ToList();

		startSize = coinText.rectTransform.localScale;

		UpdateText();

		foreach(Coin coin in coinList)
		{
			coin.Init(this);
		}
	}

	public void RemoveFromList(Coin coin)
	{
		coinCount++;
		UpdateText();
		coinList.Remove(coin);

		if(!m_duringRoutine)
			StartCoroutine(ScaleTextRoutine());
	}

	IEnumerator ScaleTextRoutine()
	{
		m_duringRoutine = true;

		float percent = 0f;
		float speed = 1f / scaleDuration;

		Vector3 targetSize = desiredSize * Vector3.one;

		while(percent < 1f)
		{
			percent += Time.deltaTime * speed;

			coinText.rectTransform.localScale = Vector3.Lerp(startSize, targetSize, scaleCurve.Evaluate(percent));

			yield return null;
		}

		m_duringRoutine = false;

	}

	void UpdateText()
	{
		coinText.SetText(coinCount.ToString() + x);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		foreach(Coin coin in coinList)
		{
			coin.transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
			//coin.transform.position = new Vector3(coin.transform.position.x,coin.transform.position.y + Mathf.Sin(Time.timeSinceLevelLoad * moveSpeed) * moveRange, coin.transform.position.z);
			coin.transform.position += Vector3.up * (Mathf.Sin(Time.timeSinceLevelLoad * moveSpeed) * moveRange);
		}
	}
}
