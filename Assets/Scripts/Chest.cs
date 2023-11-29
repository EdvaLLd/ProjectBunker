using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting System/Chest")]
public class Chest : ItemBase
{
    //beh�vs ens det h�r scriptet?
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
