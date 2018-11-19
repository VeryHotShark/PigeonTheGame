using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
	public Image playerDashScreen;

	[Header("Player Death")]

	public Image playerDeathScreen;
	
	public float deathDuration;

	public Vector2 deathMinMaxSize;

	public AnimationCurve deathScreenCurve;

	[Header("Player Health")]

	public Image playerHitScreen;

	public float showDuration;

	public AnimationCurve hitScreenCurve;

	[Space]
	public Transform healthUI;

	public Sprite aliveSprite;
	public Sprite deadSprite;

	PlayerHealth m_playerHealth;

	Image[] livesImages;

	IEnumerator m_hitScreenRoutine;

    // Use this for initialization
    void Awake()
    {
		GetComponents();

		playerHitScreen.gameObject.SetActive(false);
		playerDashScreen.gameObject.SetActive(false);
		
		livesImages = healthUI.GetComponentsInChildren<Image>().ToArray();

		//PlayerMovement.OnPlayerDash += ShowDashScreen;
    }

	void GetComponents()
	{
		m_playerHealth = FindObjectOfType<PlayerHealth>();
		m_playerHealth.OnPlayerLoseHealth += ChangeImage;
		PlayerHealth.OnPlayerDeath += ShowDeathScreen;
		m_playerHealth.OnPlayerReachCheckPoint += ResetHealthImages;
	}

    void ChangeImage(int playerCurrentHealth)
    {
		m_hitScreenRoutine = ShowHitScreen(playerHitScreen);

		if(m_hitScreenRoutine != null)
			StartCoroutine(m_hitScreenRoutine);

		if(playerCurrentHealth > 0)
		{
			int imageIndex = playerCurrentHealth;
			livesImages[imageIndex].sprite = deadSprite;
		}
		else
		{
			foreach(Image liveImage in livesImages)
			{
				liveImage.sprite = aliveSprite;
			}
		}

    }

	void ShowDashScreen()
	{
		StartCoroutine(ShowHitScreen(playerDashScreen));
	}

	void ShowDeathScreen()
	{
		StartCoroutine(ShowDeathScreenRoutine(playerDeathScreen));
	}

	void ResetHealthImages()
    {
			foreach(Image liveImage in livesImages)
			{
				liveImage.sprite = aliveSprite;
			}
    }

	IEnumerator ShowHitScreen(Image image)
	{

		float percent = 0f;
		float speed = 1f / showDuration;

		image.gameObject.SetActive(true);

		while(percent < 1f)
		{
			percent += Time.deltaTime * speed;
			var hitScreenColor = image.color;
			hitScreenColor.a = Mathf.Lerp(0f,1f, hitScreenCurve.Evaluate(percent));
			image.color = hitScreenColor;
			yield return null;
		}

		image.gameObject.SetActive(false);
	}

	IEnumerator ShowDeathScreenRoutine(Image image)
	{

		float percent = 0f;
		float speed = 1f / showDuration;

		image.gameObject.SetActive(true);

		while(percent < 1f)
		{
			percent += Time.deltaTime * speed;

			var hitScreenColor = image.color;
			hitScreenColor.a = Mathf.Lerp(0,1f, deathScreenCurve.Evaluate(percent));
			image.color = hitScreenColor;

			image.transform.localScale = Vector3.Lerp(deathMinMaxSize.x * Vector3.one, deathMinMaxSize.y  * Vector3.one, deathScreenCurve.Evaluate(percent));

			var imageEulerAngles = image.transform.localEulerAngles;
			imageEulerAngles.z = Mathf.Lerp(0f, 360f, deathScreenCurve.Evaluate(percent));
			image.transform.localEulerAngles = imageEulerAngles;

			yield return null;
		}

		image.gameObject.SetActive(false);
	}
}
