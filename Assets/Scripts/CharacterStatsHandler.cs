using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsHandler : MonoBehaviour
{

    [SerializeField]
    Slider healthSlider, hungerSlider;

    [SerializeField]
    Button medicineButton;

    [Header("Slider colors at different values")]
    [SerializeField]
    Color fullColor;
    [SerializeField]
    Color halfFullColor, lowColor;


    [SerializeField]
    Button chestBtn, legsBtn, bootsBtn, weaponBtn;

    [SerializeField]
    GameObject armorInventory;

    //private void Update()
    //{
    //    if(Inventory.GetAmountOfItem(Database.GetItemWithID("04001")) == 0)
    //    {
    //        UIManager.SetButtonIsEnabled(foodButton, false);
    //    }
    //    else
    //    {
    //        UIManager.SetButtonIsEnabled(foodButton, true);
    //    }
    //}

    public void SetUp(Character c)
    {
        healthSlider.maxValue = c.maxHealth;
        healthSlider.minValue = 0;
        hungerSlider.maxValue = c.maxHunger;
        hungerSlider.minValue = 0;

        ResetButtons(c);
        armorInventory.SetActive(false);
    }

    public void UpdateHunger(float hunger)
    {
        hungerSlider.value = hunger;
        SliderShaker(hungerSlider);
        SetColor(hungerSlider.value / hungerSlider.maxValue, hungerSlider.transform.GetChild(1).GetChild(0).gameObject);
    }
    public void UpdateHealth(float health)
    {
        healthSlider.value = health;
        SliderShaker(healthSlider);
        SetColor(healthSlider.value / healthSlider.maxValue, healthSlider.transform.GetChild(1).GetChild(0).gameObject);
    }

    void SliderShaker(Slider slider)
    {
        /*if (slider.value < slider.maxValue * 0.25f)
        {
            slider.GetComponent<Animator>().SetBool("Shaker", true);
        }
        else
        {
            slider.GetComponent<Animator>().SetBool("Shaker", false);
        }*/
    }

    void SetColor(float value, GameObject o)
    {
        if (value < .25f)
        {
            o.GetComponent<Image>().color = lowColor;
        }
        else if (value < .6f)
        {
            o.GetComponent<Image>().color = halfFullColor;
        }
        else
        {
            o.GetComponent<Image>().color = fullColor;
        }
    }

    public void UnEquipGear(Button b)
    {
        UnitController.GetSelectedCharacter().gear.UnEquipGear(b.GetComponent<EnumsToClassConverter>().GearSortingType);
        ResetButton(b, UnitController.GetSelectedCharacter());
    }


    void ResetButtons(Character c)
    {
        ResetButton(chestBtn, c);
        ResetButton(legsBtn, c);
        ResetButton(bootsBtn, c);
        ResetButton(weaponBtn, c);
    }

    public static void ResetButton(Button b, Character c)
    {
        Equipment e;
        if (c.gear.GearEquippedInSlot(out e, b.GetComponent<EnumsToClassConverter>().GearSortingType))
        {
            b.transform.GetChild(0).GetComponent<Image>().sprite = e.Icon;
            b.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            b.transform.GetChild(0).GetComponent<Image>().sprite = b.transform.GetChild(0).GetComponent<DefaultState>().defaultSprite;
            b.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}
