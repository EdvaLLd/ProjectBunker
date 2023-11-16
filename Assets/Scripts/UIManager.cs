using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject inventorySlot;
    GameObject inventoryBG;
    GameObject inventoryContentHolder;


    public SortingTypes sortingTypeEnabled = SortingTypes.All;



    private void Awake()
    {
        inventoryBG = GameObject.FindGameObjectWithTag("Inventory");
        inventoryContentHolder = GameObject.FindGameObjectWithTag("InventoryContentHolder");
    }

    private void Start()
    {
        Inventory.onInventoryUpdate += UpdateInventoryDisplay;
        inventoryBG.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            Inventory.AddItem(Database.GetItemWithID("01001"));
        }
    }

    public void ActivateWindow(GameObject windowToOpen)
    {
        windowToOpen.SetActive(!windowToOpen.active);
    }

    public void CloseWindow(GameObject windowToOpen)
    {
        windowToOpen.SetActive(false);
    }
    public void DisplayInventoryItems(EnumsToClassConverter param)
    {
        DisplayInventoryItems(param.SortingType);
    }

    public void UpdateInventoryDisplay()
    {
        DisplayInventoryItems(sortingTypeEnabled);
    }

    public void DisplayInventoryItems(SortingTypes sortingType)
    {
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
        /*for (int i = 0; i < parent.childCount; i++)
        {
            print(parent.GetChild(0).gameObject.name);
            DestroyImmediate(parent.GetChild(0).gameObject);
        }*/
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
