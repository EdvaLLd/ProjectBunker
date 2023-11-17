using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    /*static Dictionary<string, Item> itemDataBase = new Dictionary<string, Item>();
    static Dictionary<string, CraftingRecipe> recipesDataBase = new Dictionary<string, CraftingRecipe>();
    static Dictionary<string, CraftingMachine> craftingStationsDataBase = new Dictionary<string, CraftingMachine>();
    void Start()
    {
        Item[] i = FindObjectsOfTypeIncludingAssets(typeof(Item)) as Item[];
        for (int q = 0; q < i.Length; q++)
        {
            itemDataBase.Add(i[q].ID, i[q]);
        }

        CraftingRecipe[] cR = FindObjectsOfTypeIncludingAssets(typeof(CraftingRecipe)) as CraftingRecipe[];
        for (int q = 0; q < cR.Length; q++)
        {
            recipesDataBase.Add(cR[q].ID, cR[q]);
        }

        CraftingMachine[] cM = FindObjectsOfTypeIncludingAssets(typeof(CraftingMachine)) as CraftingMachine[];
        for (int q = 0; q < cM.Length; q++)
        {
            craftingStationsDataBase.Add(cM[q].ID, cM[q]);
        }
    }

    public static Item GetItemWithID(string id)
    {
        return itemDataBase.ContainsKey(id) ? itemDataBase[id] : null;
    }

    public static CraftingRecipe GetCraftingRecipeWithID(string id)
    {
        return recipesDataBase.ContainsKey(id) ? recipesDataBase[id] : null;
    }

    public static CraftingMachine GetCraftingMachineWithID(string id)
    {
        return craftingStationsDataBase.ContainsKey(id) ? craftingStationsDataBase[id] : null;
    }*/


    static Dictionary<string, Item> dataBase = new Dictionary<string, Item>();
    void Awake()
    {
        Item[] i = FindObjectsOfTypeIncludingAssets(typeof(Item)) as Item[];
        for (int q = 0; q < i.Length; q++)
        {
            dataBase.Add(i[q].ID, i[q]);
        }
    }

    public static Item GetItemWithID(string id)
    {
        return dataBase.ContainsKey(id) ? dataBase[id] : null;
    }
}
