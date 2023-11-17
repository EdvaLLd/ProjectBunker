using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestContent : MonoBehaviour
{
    Dictionary<Item, int> inventory = new Dictionary<Item, int>();

    bool looted = false;
    public void CheckContent()
    {
        if (!looted)
        {
            GenerateContent();
            Loot();
            looted = true;
        }
    }

    void GenerateContent()
    {
        inventory.Add(Database.GetItemWithID("04001"), 10);
    }

    void Loot()
    {
        if (inventory.ContainsKey(Database.GetItemWithID("04001")))
        {
            Inventory.AddItem(Database.GetItemWithID("04001"), inventory[Database.GetItemWithID("04001")]);
            inventory.Clear();
        }
    }
}
