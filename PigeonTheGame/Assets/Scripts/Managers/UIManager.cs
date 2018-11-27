using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("PopUpWinScreen")]
    public GameObject popUpScreenParent;
    public float popUpDuration;
    public float waveDuration;
    public float popUpWaitDelay;
    [Range(0f,1f)]
    public float percentTransition;
    public Vector2 popUpMinMaxSize;
    public Vector2 popUpParentMinMaxSize;
    public AnimationCurve popUpScreenCurve;
    public AnimationCurve popUpScreenCurveBG;
    public AnimationCurve popUpScreenWaveCurve;
    Image[] m_images;

    [Header("Player Win")]
    public Image playerWinScreen;
    public AnimationCurve playerWinScreenCurve;
    public float winScreenDuration;

    public Color startColor;
    public Color endColor;

    [Header("Player Death")]

    public Image playerDeathScreen;
    public Image playerDeathScreenBG;

    public float waitDelay;

    public float deathDuration;

    public Vector2 deathMinMaxSize;

    public AnimationCurve deathScreenCurve;
    public AnimationCurve deathScreenAlphaCurve;

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

    void GetComponents()
    {
        m_playerHealth = FindObjectOfType<PlayerHealth>();
        m_playerHealth.OnPlayerLoseHealth += ChangeImage;
        PlayerHealth.OnPlayerDeath += ShowDeathScreen;
        m_playerHealth.OnPlayerReachCheckPoint += ResetHealthImages;
        PlayerHealth.OnPlayerRespawn += ResetHealthImages;
        GameManager.instance.OnGameOver += Unsubscribe;

    }

    void Unsubscribe()
    {
        m_playerHealth.OnPlayerLoseHealth -= ChangeImage;
        PlayerHealth.OnPlayerDeath -= ShowDeathScreen;
        m_playerHealth.OnPlayerReachCheckPoint -= ResetHealthImages;
        PlayerHealth.OnPlayerRespawn -= ResetHealthImages;
        GameManager.instance.OnGameOver -= Unsubscribe;
    }

    void Start()
    {

        GetComponents();

        playerHitScreen.gameObject.SetActive(false);
        playerWinScreen.gameObject.SetActive(false);

        livesImages = healthUI.GetComponentsInChildren<Image>().ToArray();
        m_images = popUpScreenParent.GetComponentsInChildren<Image>();

        foreach(Image image in m_images)
        {
            image.gameObject.SetActive(false);
        }

        GameManager.instance.OnGameOver += ShowWinScreen;
    }

    void ChangeImage(int playerCurrentHealth)
    {
        m_hitScreenRoutine = ShowHitScreenRoutine(playerHitScreen,false);

        if (m_hitScreenRoutine != null)
            StartCoroutine(m_hitScreenRoutine);

        if (playerCurrentHealth >= 0)
        {
            int imageIndex = playerCurrentHealth;
            livesImages[imageIndex].sprite = deadSprite;

            if (imageIndex == 0)
            {
                livesImages[1].sprite = deadSprite;
                livesImages[2].sprite = deadSprite;
            }

            if (imageIndex == 1)
                livesImages[2].sprite = deadSprite;
        }
        else
        {
            foreach (Image liveImage in livesImages)
            {
                liveImage.sprite = aliveSprite;
            }
        }

    }

    void ShowHitScreen()
    {
        StartCoroutine(ShowHitScreenRoutine(playerHitScreen, false));
    }

    void ShowWinScreen()
    {
        StartCoroutine(ShowHitScreenRoutine(playerWinScreen, true));
    }

    void ShowDeathScreen()
    {
        StartCoroutine(ShowDeathScreenRoutine(playerDeathScreen, playerDeathScreenBG));
    }

    void ResetHealthImages()
    {
        foreach (Image liveImage in livesImages)
        {
            liveImage.sprite = aliveSprite;
        }
    }

    IEnumerator ShowHitScreenRoutine(Image image,bool winScreen)
    {

        float percent = 0f;
        float speed = 1f / (winScreen ? winScreenDuration : showDuration);

        image.gameObject.SetActive(true);

        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            var hitScreenColor = image.color;

            //if(winScreen)
                //hitScreenColor = Color.Lerp(startColor, endColor,percent);

            hitScreenColor.a = Mathf.Lerp(0f, 1f, (winScreen ? playerWinScreenCurve.Evaluate(percent) :hitScreenCurve.Evaluate(percent)));

            image.color = hitScreenColor;
            yield return null;
        }

        image.gameObject.SetActive(false);

        if(winScreen)
            StartCoroutine(ShowPopUpRoutine(0));
    }

    IEnumerator ShowPopUpRoutine(int index)
    {
        if(index == 0)
        {
            yield return new WaitForSeconds(popUpWaitDelay);
            playerDeathScreenBG.gameObject.SetActive(true);
            healthUI.gameObject.SetActive(false);
            AudioManager.instance.Play("Win");
        }

        float percent = 0f;
        float speed = 1f / popUpDuration;

        bool showNext = false;

        m_images[index].gameObject.SetActive(true);

        while(percent < 1f)
        {
            percent += Time.deltaTime * speed;

            if(index == 0)
            {
                var bgScreenColor = playerDeathScreenBG.color;
                bgScreenColor.a = Mathf.Lerp(0, 1f, popUpScreenCurveBG.Evaluate(percent));
                playerDeathScreenBG.color = bgScreenColor;
            }


            m_images[index].rectTransform.localScale = Vector3.Lerp(popUpMinMaxSize.x * Vector3.one, popUpMinMaxSize.y * Vector3.one, popUpScreenCurve.Evaluate(percent));

            if(percent >= percentTransition)
            {
                if(!showNext)
                {
                    showNext = true;
                    if(index < m_images.Length - 1)
                        StartCoroutine(ShowPopUpRoutine(index + 1));
                    else
                        StartCoroutine(WaveAnimationRoutine());
                }
            }

            yield return null;
        }
    }

    IEnumerator WaveAnimationRoutine()
    {
        float percent = 0f;
        float speed = 1f / waveDuration;

        bool fadeOut = false;

        while(percent >= 0f)
        {
            percent += Time.deltaTime * speed;

            popUpScreenParent.transform.localScale = Vector3.Lerp(popUpParentMinMaxSize.x * Vector3.one, popUpParentMinMaxSize.y * Vector3.one, popUpScreenWaveCurve.Evaluate(percent));

            if(percent > 4f && !fadeOut)
            {
                fadeOut = true;
                StartCoroutine(ReturnToMenu());
            }

            yield return null;
        }
    }

    IEnumerator ReturnToMenu()
    {
        yield return Fade.instance.StartCoroutine(Fade.instance.FadeOut());
        GameManager.instance.LoadMenu();
    }

    IEnumerator ShowDeathScreenRoutine(Image image, Image bg)
    {

        yield return new WaitForSeconds(waitDelay);

        float percent = 0f;
        float speed = 1f / deathDuration;

        image.gameObject.SetActive(true);
        bg.gameObject.SetActive(true);

        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;

            var hitScreenColor = image.color;
            hitScreenColor.a = Mathf.Lerp(0, 1f, deathScreenAlphaCurve.Evaluate(percent));
            image.color = hitScreenColor;

            image.transform.localScale = Vector3.Lerp(deathMinMaxSize.x * Vector3.one, deathMinMaxSize.y * Vector3.one, deathScreenCurve.Evaluate(percent));

            var bgScreenColor = bg.color;
            bgScreenColor.a = Mathf.Lerp(0, 1f, deathScreenAlphaCurve.Evaluate(percent));
            bg.color = bgScreenColor;

            //var imageEulerAngles = image.transform.localEulerAngles;
            //imageEulerAngles.z = Mathf.Lerp(0f, 180, deathScreenCurve.Evaluate(percent));
            //image.transform.localEulerAngles = imageEulerAngles;

            yield return null;
        }

        image.gameObject.SetActive(false);
        bg.gameObject.SetActive(false);

    }
}
