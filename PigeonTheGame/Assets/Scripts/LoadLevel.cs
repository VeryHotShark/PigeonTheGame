using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadLevel : MonoBehaviour
{

    public TextMeshProUGUI loadPercent;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(LoadLevelAsync());
    }

    string loadingString =  "Loading...";
    string percentString =  "%";

    // Update is called once per frame
    IEnumerator LoadLevelAsync()
    {

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainScene");

        while (!asyncOperation.isDone)
        {
            int percent = Mathf.CeilToInt(asyncOperation.progress * 100f);
            loadPercent.SetText("Loading... " + percent.ToString() + percentString);

            /*
            if(asyncOperation.isDone)
                SceneManager.LoadScene("Main");
            */

            yield return null;
        }
    }
}
