using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeSlot
{
    public Item item;
    public int amount;
}

[CreateAssetMenu(menuName = "Crafting System/Crafting Recipe")]
public class CraftingRecipe : ItemBase //osäker om den här ska vara ItemBase eller ScriptableObject
{
    public Item itemCrafted;
    public int itemAmount = 1;
    public List<RecipeSlot> Ingredients;
    public List<CraftingMachine> CraftableInMachine;
}
