using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    //--------Inventory--------
    [SerializeField]
    GameObject inventorySlot;
    static GameObject inventorySlotStatic;
    GameObject inventoryBG;
    GameObject inventoryContentHolder;
    Button buttonSelected;
    public SortingTypes sortingTypeEnabled = SortingTypes.All;
    //-------------------------

    public static GameObject craftingWindow;

    public static GameObject clearMistBtnGO;
    public static GameObject dangerTextGO;
    public static GameObject hoverWindow;


    public delegate void OnButtonDisableChanged(Button disabledButton);
    public static event OnButtonDisableChanged onButtonDisableChanged;


    private void Awake()
    {
        inventoryBG = GameObject.FindGameObjectWithTag("Inventory");
        inventoryContentHolder = GameObject.FindGameObjectWithTag("InventoryContentHolder");
        craftingWindow = GameObject.FindGameObjectWithTag("CraftingWindow");
        clearMistBtnGO = GameObject.FindGameObjectWithTag("ClearMistBtn");
        if (inventoryBG != null)
        {
            buttonSelected = inventoryBG.transform.GetChild(0).GetChild(0).GetComponent<Button>();
        }
        dangerTextGO = GameObject.FindGameObjectWithTag("DangerTxt");
        hoverWindow = GameObject.FindGameObjectWithTag("HoverWindow");

        inventorySlotStatic = inventorySlot;
    }

    private void Start()
    {
        Inventory.onInventoryUpdate += UpdateInventoryDisplay;
        inventoryBG.SetActive(false);
        craftingWindow.SetActive(false);
        clearMistBtnGO.SetActive(false);
        dangerTextGO.SetActive(false);
        hoverWindow.SetActive(false);
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
    //kanske ett d�ligt namn p� den h�r men orkar inte koppla om alla knappar till en metod med annat namn
    public static void ActivateWindow(GameObject windowToOpen)
    {
        windowToOpen.SetActive(!windowToOpen.active);
    }
    //
    public static void SetWindowActive(GameObject windowToOpen)
    {
        windowToOpen.SetActive(true);
    }
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
        HelperMethods.ClearChilds(parent);
        CreateInventory(sortingType, inventoryContentHolder);
    }

    public static GameObject CreateInventory(SortingTypes type, GameObject inventoryHolder)
    {
        if (type == SortingTypes.All)
        {
            foreach (KeyValuePair<Item, int> item in Inventory.inventory)
            {
                AddSlot(inventoryHolder.transform, item.Key, item.Value);
            }
        }
        else
        {
            Dictionary<Item, int> temp = Inventory.GetItemsOfType(type);
            foreach (KeyValuePair<Item, int> item in temp)
            {
                AddSlot(inventoryHolder.transform, item.Key, item.Value);
            }
        }
        return inventoryHolder;
    }

    public static GameObject CreateInventory(GearTypes type, GameObject inventoryHolder)
    {
        Dictionary<Item, int> temp = Inventory.GetGearOfType(type);
        foreach (KeyValuePair<Item, int> item in temp)
        {
            AddSlot(inventoryHolder.transform, item.Key, item.Value);
        }
        return inventoryHolder;
    }

    static GameObject AddSlot(Transform parent, Item item, int amount)
    {
        GameObject t = Instantiate(inventorySlotStatic, parent);
        t.GetComponent<ItemHoverDesc>().item = item;
        t.GetComponent<Image>().sprite = item.Icon;
        t.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = amount.ToString();
        return t;
    }
}
