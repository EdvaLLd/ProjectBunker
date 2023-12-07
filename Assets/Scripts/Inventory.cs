using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Inventory
{
    public static Dictionary<Item, int> inventory {get; } = new Dictionary<Item, int>(); //Item|num
    public static Dictionary<CraftingMachine, List<CraftingRecipe>> recipesUnlocked {get; } = new Dictionary<CraftingMachine, List<CraftingRecipe>>(); //Machine|Recipes

    public delegate void OnInventoryUpdate();
    public static event OnInventoryUpdate onInventoryUpdate;

    public delegate void OnRecipeAdded(CraftingRecipe recipe);
    public static event OnRecipeAdded onRecipeAdded;

    /*public static void AddItem(Item item)
    {
        inventory.Add(item, 1);

        onInventoryUpdate?.Invoke();
    }*/

    public static void AddItem(Item item, int amount = 1)
    {
        TextLog.AddLog($"Obtained {amount} {item.DisplayName}!", MessageTypes.looted);
        if(inventory.ContainsKey(item)) inventory[item] += amount;
        else inventory.Add(item, amount);
        onInventoryUpdate?.Invoke();
    }

    public static bool IsCraftable(CraftingRecipe recipe)
    {
        for (int i = 0; i < recipe.Ingredients.Count; i++)
        {
            if (GetAmountOfItem(recipe.Ingredients[i].item) < recipe.Ingredients[i].amount) return false;
        }
        return true;
    }

    public static bool IsCraftable(CraftingRecipe recipe, int amount)
    {
        for (int i = 0; i < recipe.Ingredients.Count; i++)
        {
            if (GetAmountOfItem(recipe.Ingredients[i].item) < recipe.Ingredients[i].amount * amount) return false;
        }
        return true;
    }

    public static int MaxAmountCraftable(CraftingRecipe recipe)
    {
        int amount = GetAmountOfItem(recipe.Ingredients[0].item) / recipe.Ingredients[0].amount;
        for (int i = 1; i < recipe.Ingredients.Count; i++)
        {
            int t = GetAmountOfItem(recipe.Ingredients[i].item) / recipe.Ingredients[i].amount;
            if (t < amount) amount = t;
        }
        return amount;
    }

    public static bool IsRecipeCraftableInMachine(CraftingMachine machine, CraftingRecipe recipe)
    {
        List<CraftingRecipe> recipesInMachine = GetRecipesForMachine(machine);
        foreach (CraftingRecipe r in recipesInMachine)
        {
            if (r == recipe) return true;
        }
        return false;
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
            TextLog.AddLog($"Learned recipe: {recipe.itemCrafted.DisplayName}!", MessageTypes.specialItem);
            onRecipeAdded?.Invoke(recipe);
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
        if (inventory.ContainsKey(item)) { return inventory[item]; }
        return 0;
    }

    public static Dictionary<Item, int> GetItemsOfType(SortingTypes type)
    {
        Dictionary<Item, int> temp = new Dictionary<Item, int>(); //Item|num
        foreach (KeyValuePair<Item, int> item in inventory)
        {
            if (item.Key.SortingType == type)
            {
                temp.Add(item.Key, item.Value);
            }
        }
        return temp;
    }

    public static Dictionary<Item, int> GetGearOfType(GearTypes type)
    {
        Dictionary<Item, int> temp = new Dictionary<Item, int>(); //Item|num
        foreach (KeyValuePair<Item, int> item in inventory)
        {
            if (item.Key.SortingType == SortingTypes.Gear && (item.Key as Equipment).gearType == type)
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
            TextLog.AddLog($"Removed {amount} {item.DisplayName} from inventory!", MessageTypes.used);
            inventory[item] -= amount;
            if (inventory[item] == 0) inventory.Remove(item);
            onInventoryUpdate?.Invoke();
            return true;
        }
        return false;
    }
}
