using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCraftingMachine : InteractableItem
{
    [SerializeField]
    GameObject craftingWindow;
    [SerializeField]
    CraftingMachine machine;

    public override void InteractWith()
    {
        GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().ActivateWindow(craftingWindow);
        if (craftingWindow.active)
        {
            print("Window opened");
            craftingWindow.GetComponent<CraftingWindow>().InitCraftingWindow(machine);
        }
        
    }
}
