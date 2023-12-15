using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting System/Chest")]
public class Chest : ItemBase
{
    //behövs ens det här scriptet?
    [SerializeField]
    Dictionary<Item, int> inventory = new Dictionary<Item, int>();

    bool looted = false;

    public void CheckContent()
    {
        if(!looted)
        {
            Loot();
            looted = true;
        }
    }

    void Loot()
    {
        TextLog.AddLog("Looted chest contains:");
        foreach (KeyValuePair<Item, int> item in inventory)
        {
            Inventory.AddItem(item.Key, item.Value);
        }
    }
}
