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

    public void SetUp()
    {
        healthSlider.maxValue = 100;
        healthSlider.minValue = 0;
        hungerSlider.maxValue = 100;
        hungerSlider.minValue = 0;
    }

    public void UpdateHunger(float hunger)
    {
        hungerSlider.value = hunger;
    }
    public void UpdateHealth(float health)
    {
        healthSlider.value = health;
    }
}
