using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Inventory
{
    public static Dictionary<Item, int> inventory {get; } = new Dictionary<Item, int>(); //Item|num

    public delegate void OnInventoryUpdate();
    public static event OnInventoryUpdate onInventoryUpdate;

    /*public static void AddItem(Item item)
    {
        inventory.Add(item, 1);

        onInventoryUpdate?.Invoke();
    }*/

    public static void AddItem(Item item, int amount = 1)
    {
        if(inventory.ContainsKey(item)) inventory[item] += amount;
        else inventory.Add(item, amount);
        onInventoryUpdate?.Invoke();
    }

    public static void AddItem(string ID)
    {
        //Skulle tro att vi vill ha en sån här om man fixar en bättre databas för items?
    }

    public static int GetAmountOfItem(Item item)
    {
        if(inventory.ContainsKey(item)) return inventory[item];
        return 0;
    }

    public static Dictionary<Item, int> GetItemsOfType(SortingTypes type)
    {
        Dictionary<Item, int> temp = new Dictionary<Item, int>(); //Item|num
        foreach (KeyValuePair<Item, int> item in inventory)
        {
            if(item.Key.SortingType == type)
            {
                temp.Add(item.Key, item.Value);
            }
        }
        return temp;
    }

    public static bool RemoveItem(Item item, int amount = 1)
    {
        if (inventory.ContainsKey(item) && inventory[item] >= amount)
        {
            inventory[item] -= amount;
            if (inventory[item] == 0) inventory.Remove(item);
            onInventoryUpdate?.Invoke();
            return true;
        }
        return false;
    }
}
