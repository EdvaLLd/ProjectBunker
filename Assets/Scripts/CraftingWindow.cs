using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//det h�r f�nstret borde resettas n�r man st�nger det (iaf den lilla "machinebg")
public class CraftingWindow : MonoBehaviour
{
    [SerializeField]
    GameObject recipePrefab, ingredientPrefab;

    [SerializeField]
    GameObject recipeList;

    [SerializeField]
    GameObject craftingWindow;

    CraftingRecipe recipeMarked;
    public void InitCraftingWindow(CraftingMachine machine)
    {
        Time.timeScale = 0;
        ClearChilds(recipeList.transform);
        List<CraftingRecipe> recipesForMachine = Inventory.GetRecipesForMachine(machine);
        for (int i = 0; i < recipesForMachine.Count; i++)
        {
            CraftingRecipe recipe = recipesForMachine[i];
            GameObject t;
            t = Instantiate(recipePrefab, recipeList.transform);
            t.transform.GetChild(0).GetComponent<Image>().sprite = recipe.Icon;
            t.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = recipe.DisplayName;
            t.GetComponent<Button>().onClick.AddListener(() => RecipeClicked(recipe));
            craftingWindow.GetComponent<Image>().sprite = machine.Icon;
            transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = machine.name;
            InitRecipeWindow(t, recipe);
        }
    }

    public void UnPause()
    {
        Time.timeScale = 1;
    }

    void InitRecipeWindow(GameObject window, CraftingRecipe recipe)
    {
        GameObject ingredientList = window.transform.GetChild(2).gameObject; //super duper scary s�tt att fixa p�
        for (int i = 0; i < recipe.Ingredients.Count; i++)
        {
            GameObject t;
            t = Instantiate(ingredientPrefab, ingredientList.transform);
            t.GetComponent<Image>().sprite = recipe.Ingredients[i].item.Icon;
            t.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = recipe.Ingredients[i].amount.ToString();
        }
    }

    void ClearChilds(Transform parent)
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void RecipeClicked(CraftingRecipe recipe)
    {
        if(IsCraftable(recipe))
        {
            recipeMarked = recipe;
            craftingWindow.SetActive(true);
            craftingWindow.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = recipe.DisplayName;
        }
    }

    public void CraftItem()
    {
        if(IsCraftable(recipeMarked)) //kanske on�dig, men k�nns bra att dubbelkolla
        {
            for (int i = 0; i < recipeMarked.Ingredients.Count; i++)
            {
                Inventory.RemoveItem(recipeMarked.Ingredients[i].item, recipeMarked.Ingredients[i].amount);
            }
            Inventory.AddItem(recipeMarked.itemCrafted, recipeMarked.itemAmount);
        }
    }

    bool IsCraftable(CraftingRecipe recipe)
    {
        for (int i = 0; i < recipe.Ingredients.Count; i++)
        {
            if (Inventory.GetAmountOfItem(recipe.Ingredients[i].item) < recipe.Ingredients[i].amount) return false;
        }
        return true;
    }

    /*CraftingRecipe GetRecipeByID(string ID)  //Beh�vs nog inte
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            if (recipes[i].ID == ID) return recipes[i];
        }
        print("Shouldnt be here");
        return null;
    }*/

}
