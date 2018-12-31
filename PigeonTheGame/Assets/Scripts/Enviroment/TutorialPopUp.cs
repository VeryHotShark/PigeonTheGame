using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopUp : MonoBehaviour
{

    public GameObject tutorialScreen;
    public Sprite tutorialImage;

    Image tutorialScreenImage;

    void Start()
    {
        tutorialScreenImage = tutorialScreen.GetComponent<Image>();
    }

    // Use this for initialization
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialScreen.SetActive(true);
            tutorialScreenImage.sprite = tutorialImage;
        }
    }

    // Update is called once per frame
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialScreen.SetActive(false);
            tutorialScreenImage.sprite = null;
        }
    }
}
