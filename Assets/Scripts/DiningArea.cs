using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiningArea : InteractableItem
{
    [SerializeField]
    GameObject diningUI;
    [SerializeField]
    GameObject diningInventory;
    [SerializeField]
    GameObject diningText;

    Character character;

    private void Awake()
    {
        Inventory.onInventoryUpdate += InitInventory;
    }

    public void OpenUI(Character character)
    {
        this.character = character;
        UIManager.SetWindowActive(diningUI);
        InitInventory();
    }


    void InitInventory()
    {
        HelperMethods.ClearChilds(diningInventory.transform);
        diningText.GetComponent<TextMeshProUGUI>().text = item.DisplayName;
        Dictionary<Item, int> allFoods = Inventory.GetItemsOfType(SortingTypes.Food);
        foreach (KeyValuePair<Item, int> item in allFoods)
        {
            InitSlot(item.Key, item.Value);
        }
    }

    void InitSlot(Item item, int amount)
    {
        GameObject slot = UIManager.InitInventorySlot(item, amount, diningInventory.transform);
        Button b = slot.AddComponent<Button>();
        b.onClick.AddListener(() => UnitController.FeedCharacter(item as Food, character));
    }
}
