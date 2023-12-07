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
    public static GameObject canvas;

    public static float canvasScaleFactor;


    public delegate void OnButtonDisableChanged(Button disabledButton);
    public static event OnButtonDisableChanged onButtonDisableChanged;

    public delegate void OnCanvasScaleFactorChange();
    public static event OnCanvasScaleFactorChange onCanvasScaleFactorChange;

    RaycastHit[] gameObjectHoveredLastFrame;


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
        canvas = GameObject.FindGameObjectWithTag("Canvas");

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

        //kanske problematiskt att den här bara körs en gång?
        canvasScaleFactor = Camera.main.scaledPixelWidth / canvas.GetComponent<CanvasScaler>().referenceResolution.x;
    }

    
    private void FixedUpdate()
    {
        Vector3 start = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0,0, Camera.main.transform.position.z));
        Debug.DrawRay(start, Vector3.forward * 10, Color.red, 5);
        //start.z = Camera.main.transform.position.z;
        RaycastHit[] objectsHovered = Physics.RaycastAll(start, Vector3.forward, float.PositiveInfinity);
        /*foreach (RaycastHit item in objectsHovered)
        {
            print(item.collider.name);
        }
        print(objectsHovered.Length);*/
    }

    void SetCanvasScale()
    {
        float newScale = Camera.main.scaledPixelWidth / canvas.GetComponent<CanvasScaler>().referenceResolution.x;
        if(newScale != canvasScaleFactor)
        {
            canvasScaleFactor = newScale;
            onCanvasScaleFactorChange?.Invoke();
        }
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
    //kanske ett dåligt namn på den här men orkar inte koppla om alla knappar till en metod med annat namn
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

    public static void MoveObjectToScreen(GameObject window)
    {
        Vector2 cameraBounds = new Vector2(Camera.main.scaledPixelWidth, Camera.main.scaledPixelHeight);
        //print(test);
        RectTransform rt = window.GetComponent<RectTransform>();
        Vector2 windowSize = (rt.anchorMax - rt.anchorMin + rt.sizeDelta) * canvasScaleFactor;
        Vector4 windowBounds = new Vector4(rt.position.x + windowSize.x/2, rt.position.x - windowSize.x / 2, 
            rt.position.y + windowSize.y / 2, rt.position.y - windowSize.y / 2);
        //float difInPos = rt.position.x + windowSize.x - cameraBounds.x;
        //print(windowBounds.x);
        if (windowBounds.x > cameraBounds.x)
        {
            window.transform.position += new Vector3(cameraBounds.x - windowBounds.x, 0, 0);
            print("move left " + (cameraBounds.x - windowBounds.x).ToString());
        }
        //difInPos = rt.position.x - windowSize.x + cameraBounds.x;
        else if (windowBounds.y < 0)
        {
            window.transform.position -= new Vector3(windowBounds.y, 0, 0);
            print("move right " + windowBounds.y.ToString());
        }

        //difInPos = rt.position.y + windowSize.y - cameraBounds.y;
        if (windowBounds.z > cameraBounds.y)
        {
            window.transform.position += new Vector3(0, cameraBounds.y - windowBounds.z, 0);
            print("move down " + (cameraBounds.y - windowBounds.z).ToString());
        }
        //difInPos = rt.position.y - windowSize.y + cameraBounds.y;
        else if (windowBounds.w < 0)
        {
            window.transform.position -= new Vector3(0, windowBounds.w, 0);
            print("move up");
        }
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
