using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayGear : MonoBehaviour
{
    Button buttonClicked;
    public void DisplayGearOfType(Button button)
    {
        buttonClicked = button;
        GearTypes type = button.GetComponent<EnumsToClassConverter>().GearSortingType;
        HelperMethods.ClearChilds(transform);
        UIManager.CreateInventory(type, gameObject);
        print(transform.childCount);
        foreach (Transform child in transform)
        {
            if (!child.TryGetComponent<Button>(out Button t))
            {
                Button b = child.gameObject.AddComponent<Button>();
                b.onClick.AddListener(() => EquipGear(child.GetComponent<ItemHoverDesc>().item as Equipment));
            }
        }
        /*for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).TryGetComponent<Button>(out Button t))
            {
                Button b = transform.GetChild(i).gameObject.AddComponent<Button>();
                b.onClick.AddListener(() => EquipGear(transform.GetChild(i).GetComponent<ItemHoverDesc>().item as Equipment));
            }
        }*/
    }

    public void EquipGear(Equipment gear)
    {
        buttonClicked.transform.GetChild(0).GetComponent<Image>().sprite = gear.Icon;
        UnitController.GetSelectedCharacter().EquipGear(gear);
    }
}
