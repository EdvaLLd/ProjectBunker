using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheatUI : MonoBehaviour
{
    [SerializeField]
    GameObject buttonPrefab;
    [SerializeField]
    GameObject items, recipes, holder;


    CraftingRecipe[] r;

    [SerializeField]
    GameObject character;
    [SerializeField]
    GameObject spawnPos;

    private void Start()
    {
        Item[] i = FindObjectsOfTypeIncludingAssets(typeof(Item)) as Item[];
        foreach (Item item in i)
        {
            GameObject t = Instantiate(buttonPrefab, items.transform);
            t.GetComponent<Button>().onClick.AddListener(() => AddItem(item));
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.DisplayName;
        }
        r = FindObjectsOfTypeIncludingAssets(typeof(CraftingRecipe)) as CraftingRecipe[];
        foreach (CraftingRecipe craftingRecipe in r)
        {
            GameObject t = Instantiate(buttonPrefab, recipes.transform);
            t.GetComponent<Button>().onClick.AddListener(() => AddRecipe(craftingRecipe));
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = craftingRecipe.itemCrafted.DisplayName;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F3))
        {
            UIManager.ActivateWindow(holder);
            if(holder.active)
            {
                HelperMethods.ClearChilds(recipes.transform);
                foreach (CraftingRecipe craftingRecipe in r)
                {
                    Dictionary<CraftingMachine, List<CraftingRecipe>> recipesUnlocked = Inventory.recipesUnlocked;
                    bool add = false;
                    if (recipesUnlocked.ContainsKey(craftingRecipe.CraftableInMachine[0]))
                    {
                        if (!recipesUnlocked[craftingRecipe.CraftableInMachine[0]].Contains(craftingRecipe))
                        {
                            add = true;
                        }
                    }
                    else
                    {
                        add = true;
                    }
                    if(add)
                    {
                        GameObject t = Instantiate(buttonPrefab, recipes.transform);
                        t.GetComponent<Button>().onClick.AddListener(() => AddRecipe(craftingRecipe));
                        t.GetComponent<Button>().onClick.AddListener(() => Destroy(t));
                        t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = craftingRecipe.itemCrafted.DisplayName;
                    }
                }
            }
        }
    }

    public void AddCharacter()
    {
        GameObject temp = Instantiate(character, spawnPos.transform.position, Quaternion.identity);
        temp.transform.GetChild(0).gameObject.SetActive(true);

        temp.GetComponentInChildren<PartsChanger>().RandomizeCharacter();
        UnitController.AddCharacter(temp.GetComponent<Character>());
    }

    public void AddItem(Item item)
    {
        Inventory.AddItem(item);
    }
    public void AddRecipe(CraftingRecipe recipe)
    {
        Inventory.AddRecipeToMachines(recipe);
    }
}
