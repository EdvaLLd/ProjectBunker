using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class TutorialImage
{
    public Sprite image;
    public string text;
    public bool hasImage = true;
}

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    Button b1, b2, changeScene;

    [SerializeField]
    TextMeshProUGUI text, slideNRText;

    [SerializeField]
    Image image;


    [SerializeField]
    TutorialImage[] tutorialImages;

    int currentSlide = 0;

    public void ChangeSlide(int amount)
    {
        currentSlide += amount;

        if(currentSlide == tutorialImages.Length)
        {
            ChangeScene("MainScene");
            return;
        }

        if (currentSlide == 0) b1.interactable = false;
        else b1.interactable = true;

        if(currentSlide == tutorialImages.Length - 1)
        {
            b2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Finish tutorial";
            b2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 15;
            changeScene.gameObject.SetActive(false);
        }
        else
        {
            b2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ">";
            b2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 60;
            changeScene.gameObject.SetActive(true);
        }
        slideNRText.text = (currentSlide+1).ToString() + "/" + tutorialImages.Length.ToString();
        text.text = tutorialImages[currentSlide].text;
        if (tutorialImages[currentSlide].hasImage) image.gameObject.SetActive(true);
        else image.gameObject.SetActive(false);
        image.sprite = tutorialImages[currentSlide].image;
        image.SetNativeSize();
    }

    public void ChangeScene(string newScene)
    {
        SceneManager.LoadScene(newScene);
    }

    private void Start()
    {
        ChangeSlide(0);
    }
}
