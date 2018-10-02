using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{

	[Header("Player Health")]

	public Transform healthUI;

	public Sprite aliveSprite;
	public Sprite deadSprite;

	PlayerHealth m_playerHealth;

	Image[] livesImages;

    // Use this for initialization
    void Awake()
    {
		GetComponents();
		livesImages = healthUI.GetComponentsInChildren<Image>().ToArray();
    }

	void GetComponents()
	{
		m_playerHealth = FindObjectOfType<PlayerHealth>();
		m_playerHealth.OnPlayerLoseHealth += ChangeImage;
	}

    // Update is called once per frame
    void ChangeImage(int playerCurrentHealth)
    {
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
}
