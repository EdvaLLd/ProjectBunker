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

    float scale;

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
            changeScene.gameObject.SetActive(false);
        }
        else
        {
            b2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ">";
            changeScene.gameObject.SetActive(true);
        }
        slideNRText.text = (currentSlide+1).ToString() + "/" + tutorialImages.Length.ToString();
        text.text = tutorialImages[currentSlide].text;
        image.sprite = tutorialImages[currentSlide].image;
        image.SetNativeSize();
    }

    public void ChangeScene(string newScene)
    {
        SceneManager.LoadScene(newScene);
    }

    private void Start()
    {
        scale = Camera.main.scaledPixelWidth / GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasScaler>().referenceResolution.x;
        ChangeSlide(0);
    }
}
