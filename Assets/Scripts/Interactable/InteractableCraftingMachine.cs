using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCraftingMachine : InteractableItem
{
    /*GameObject craftingWindow;
    //[SerializeField]
    //CraftingMachine machine;

    private void Awake()
    {
        craftingWindow = GameObject.FindGameObjectWithTag("CraftingWindow");
    }
    private void Start()
    {
        craftingWindow.SetActive(false);
    }

    public override void InteractWith()
    {
        GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().ActivateWindow(craftingWindow);
        if (craftingWindow.active)
        {
            print("Window opened");
            craftingWindow.GetComponent<CraftingWindow>().InitCraftingWindow(item as CraftingMachine);
        } 
    }*/
}
