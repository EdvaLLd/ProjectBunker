using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Inventory
{
    public static Dictionary<Item, int> inventory {get; } = new Dictionary<Item, int>(); //Item|num
    public static Dictionary<CraftingMachine, List<CraftingRecipe>> recipesUnlocked {get; } = new Dictionary<CraftingMachine, List<CraftingRecipe>>(); //Machine|Recipes

    public delegate void OnInventoryUpdate();
    public static event OnInventoryUpdate onInventoryUpdate;

    /*public static void AddItem(Item item)
    {
        inventory.Add(item, 1);

        onInventoryUpdate?.Invoke();
    }*/

    public static void AddItem(Item item, int amount = 1)
    {
        TextLog.AddLog($"Obtained {amount} {item.DisplayName}!", Color.green);
        if(inventory.ContainsKey(item)) inventory[item] += amount;
        else inventory.Add(item, amount);
        onInventoryUpdate?.Invoke();
    }


    public static void AddRecipeToMachines(CraftingRecipe recipe)
    {
        bool recipeAdded = false;
        for (int i = 0; i < recipe.CraftableInMachine.Count; i++)
        {
            if (recipesUnlocked.ContainsKey(recipe.CraftableInMachine[i]))
            {
                if (!recipesUnlocked[recipe.CraftableInMachine[i]].Contains(recipe))
                {
                    recipesUnlocked[recipe.CraftableInMachine[i]].Add(recipe);
                    recipeAdded = true;
                }
            }
            else
            {
                recipesUnlocked.Add(recipe.CraftableInMachine[i], new List<CraftingRecipe>() { recipe });
                recipeAdded = true;
            }
        }
        if(recipeAdded)
        {
            TextLog.AddLog($"Learned recipe: {recipe.DisplayName}!", Color.yellow);
        }
    }

    public static List<CraftingRecipe> GetRecipesForMachine(CraftingMachine machine)
    {
        if(recipesUnlocked.ContainsKey(machine)) return recipesUnlocked[machine];
        else return new List<CraftingRecipe>();
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
            TextLog.AddLog($"Removed {amount} {item.DisplayName} from inventory!", Color.red);
            inventory[item] -= amount;
            if (inventory[item] == 0) inventory.Remove(item);
            onInventoryUpdate?.Invoke();
            return true;
        }
        return false;
    }
}
