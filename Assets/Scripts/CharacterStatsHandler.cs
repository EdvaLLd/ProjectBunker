using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsHandler : MonoBehaviour
{

    [SerializeField]
    Slider healthSlider, hungerSlider;

    [SerializeField]
    Button foodButton;

    [Header("Slider colors at different values")]
    [SerializeField]
    Color fullColor;
    [SerializeField]
    Color halfFullColor, lowColor;

    private void Update()
    {
        if(Inventory.GetAmountOfItem(Database.GetItemWithID("04001")) == 0)
        {
            foodButton.interactable = false;
        }
        else
        {
            foodButton.interactable = true;
        }
    }

    public void SetUp(Character c)
    {
        healthSlider.maxValue = c.maxHealth;
        healthSlider.minValue = 0;
        hungerSlider.maxValue = c.maxHunger;
        hungerSlider.minValue = 0;
    }

    public void UpdateHunger(float hunger)
    {
        hungerSlider.value = hunger;
        SetColor(hungerSlider.value / hungerSlider.maxValue, hungerSlider.transform.GetChild(1).GetChild(0).gameObject);
    }
    public void UpdateHealth(float health)
    {
        healthSlider.value = health;

        SetColor(healthSlider.value / healthSlider.maxValue, healthSlider.transform.GetChild(1).GetChild(0).gameObject);
    }

    void SetColor(float value, GameObject o)
    {
        if(value < .25f)
        {
            o.GetComponent<Image>().color = lowColor;
        }
        else if(value < .6f)
        {
            o.GetComponent<Image>().color = halfFullColor;
        }
        else
        {
            o.GetComponent<Image>().color = fullColor;
        }
    }
}
