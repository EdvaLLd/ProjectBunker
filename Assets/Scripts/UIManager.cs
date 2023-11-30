using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    //--------Inventory--------
    [SerializeField]
    GameObject inventorySlot;
    GameObject inventoryBG;
    GameObject inventoryContentHolder;
    Button buttonSelected;
    public SortingTypes sortingTypeEnabled = SortingTypes.All;
    //-------------------------

    public static GameObject craftingWindow;

    public static GameObject clearMistBtnGO;


    public delegate void OnButtonDisableChanged(Button disabledButton);
    public static event OnButtonDisableChanged onButtonDisableChanged;


    private void Awake()
    {
        inventoryBG = GameObject.FindGameObjectWithTag("Inventory");
        inventoryContentHolder = GameObject.FindGameObjectWithTag("InventoryContentHolder");
        craftingWindow = GameObject.FindGameObjectWithTag("CraftingWindow");
        clearMistBtnGO = GameObject.FindGameObjectWithTag("ClearMistBtn");
        buttonSelected = inventoryBG.transform.GetChild(0).GetChild(0).GetComponent<Button>();
    }

    private void Start()
    {
        Inventory.onInventoryUpdate += UpdateInventoryDisplay;
        inventoryBG.SetActive(false);
        craftingWindow.SetActive(false);
        clearMistBtnGO.SetActive(false);

        Inventory.AddItem(Database.GetItemWithID("01001"), 5);
        Inventory.AddItem(Database.GetItemWithID("01002"), 1);
    }

    public static void SetButtonIsEnabled(Button button, bool value)
    {
        if(value != button.interactable)
        {
            button.interactable = value;
            onButtonDisableChanged?.Invoke(button);
        }
    }

    //Emma
    public static void ActivateWindow(GameObject windowToOpen)
    {
        windowToOpen.SetActive(!windowToOpen.active);
    }
    //
    public static void CloseWindow(GameObject windowToOpen)
    {
        UIElementConsumeMouseOver.mouseOverIsAvailable = true;
        windowToOpen.SetActive(false);
    }
    public void DisplayInventoryItems(Button button)
    {
        EnumsToClassConverter temp = button.GetComponent<EnumsToClassConverter>();
        DisplayInventoryItems(temp.SortingType, button);
    }

    public void UpdateInventoryDisplay()
    {
        DisplayInventoryItems(buttonSelected);
    }

    
    public void DisplayInventoryItems(SortingTypes sortingType, Button button)
    {
        SetButtonIsEnabled(buttonSelected, true);
        SetButtonIsEnabled(button, false);
        buttonSelected = button;

        sortingTypeEnabled = sortingType;
        Transform parent = inventoryContentHolder.transform;
        ClearInventory(parent);
        if (sortingType == SortingTypes.All)
        {
            foreach (KeyValuePair<Item, int> item in Inventory.inventory)
            {
                AddSlot(parent, item.Key, item.Value);
            }
        }
        else
        {
            Dictionary<Item, int> temp = Inventory.GetItemsOfType(sortingType);
            foreach (KeyValuePair<Item, int> item in temp)
            {
                AddSlot(parent, item.Key, item.Value);
            }
        }
    }
    void AddSlot(Transform parent, Item item, int amount)
    {
        GameObject t = Instantiate(inventorySlot, parent);
        t.GetComponent<Image>().sprite = item.Icon;
        t.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = amount.ToString();
    }

    void ClearInventory(Transform parent)
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
