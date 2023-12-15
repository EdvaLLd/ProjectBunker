using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCheckColorController : MonoBehaviour
{

    private float dayValue;

    [SerializeField] private GameObject clouds;

    [SerializeField] private Image background;

    public Color dayColor;
    public Color nightColor;
    public Color currentColor;

    public Color backgroundDayColor;
    public Color backgroundNightColor;

    // Start is called before the first frame update
    void Start()
    {
        currentColor = dayColor;
    }

    // Update is called once per frame
    void Update()
    {
        dayValue = gameObject.GetComponent<SkyboxController>().dayNightValue;
        
        // Day 0 - 180


        /*
        if (dayValue >= 0 && dayValue <= 180) {
            // Day
            // clouds.GetComponent<Renderer>().material.SetColor("_CloudsColor", dayColor);
            clouds.GetComponent<Renderer>().material.SetColor("_CloudsColor", currentColor);
        }

        else if (dayValue >= 181 && dayValue <= 360) {
            // Night
            clouds.GetComponent<Renderer>().material.SetColor("_CloudsColor", currentColor);
        }
        */

        if (dayValue >= 170 && dayValue <= 190) {
            currentColor = Color.Lerp(dayColor, nightColor, (dayValue-170)/20);
            clouds.GetComponent<Renderer>().material.SetColor("_CloudsColor", currentColor);
            background.GetComponent<Image>().color = Color.Lerp(backgroundDayColor, backgroundNightColor, (dayValue-170)/20);
        }

        if (dayValue >= 0 && dayValue <= 20) {
            currentColor = Color.Lerp(nightColor, dayColor, dayValue/20);
            clouds.GetComponent<Renderer>().material.SetColor("_CloudsColor", currentColor);
            background.GetComponent<Image>().color = Color.Lerp(backgroundNightColor, backgroundDayColor, dayValue/20);
        }
    }
}
