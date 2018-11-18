using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{

	public Image playerDashScreen;

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
}
