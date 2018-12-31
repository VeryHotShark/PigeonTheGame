using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{

    public static Fade instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public enum fade { FadeIn, FadeOut, }
    public fade fadeType;
    public Image background;

    public float fadeDuration;

    void Start()
    {
        switch (fadeType)
        {
            case fade.FadeIn:
                StartCoroutine(FadeIn());
                break;

            case fade.FadeOut:
                StartCoroutine(FadeOut());
                break;
        }
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

	  public void StartFadeOutLoadScene()
    {
        StartCoroutine(GoToGame());
    }


    public IEnumerator FadeIn()
    {
        float fadeSpeed = 1f / fadeDuration;
        float percent = 0f;

        while (percent <= 1f)
        {
            percent += Time.deltaTime * fadeSpeed;
            background.color = Color.Lerp(Color.black, Color.clear, percent);
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        float fadeSpeed = 1f / fadeDuration;
        float percent = 0f;

        while (percent <= 1f)
        {
            percent += Time.deltaTime * fadeSpeed;
            background.color = Color.Lerp(Color.clear, Color.black, percent);
            yield return null;
        }
    }

	public IEnumerator GoToGame()
    {
        yield return Fade.instance.StartCoroutine(Fade.instance.FadeOut());
        MainMenuManager.instance.StartGame();
    }

    public void QuitGame()
    {
        MainMenuManager.instance.QuitGame();
    }
}
