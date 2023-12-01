using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//det här fönstret borde resettas när man stänger det (iaf den lilla "machinebg")
public class CraftingWindow : MonoBehaviour
{
    [SerializeField]
    GameObject recipePrefab, ingredientPrefab;

    [SerializeField]
    GameObject recipeList;

    [SerializeField]
    GameObject craftingWindow, recipeImg, recipeName;

    CraftingRecipe recipeMarked;

    InteractableCraftingMachine craftingMachine;

    [SerializeField]
    Button lowerAmountBtn, increaseAmountBtn;

    [SerializeField]
    GameObject amountTxt;
    [SerializeField]
    Slider progressSlider, amountSlider;

    Character characterWhoOpenedWindow;

    private void Start()
    {
        Inventory.onInventoryUpdate += UpdateAmountSliderValues;
        Inventory.onRecipeAdded += NewRecipeAdded;
    }

    void NewRecipeAdded(CraftingRecipe recipe)
    {
        print("New recipe!");
        if(Inventory.IsRecipeCraftableInMachine(craftingMachine.item as CraftingMachine, recipe))
        {
            print("Affects current machine");
            InitCraftingWindow(craftingMachine.item as CraftingMachine, craftingMachine, characterWhoOpenedWindow);
        }
    }

    public void InitCraftingWindow(CraftingMachine machine, InteractableCraftingMachine physicalMachine, Character character)
    {
        craftingMachine = physicalMachine;
        characterWhoOpenedWindow = character;
        //Time.timeScale = 0;
        ClearChilds(recipeList.transform);
        if(physicalMachine.GetProgress() != 0) //kommer inte det här göra så om man timear och avbryter tasken när timern är på EXAKT 0 så resettas den??
        {
            RecipeClicked(physicalMachine.GetRecipe());
            SetCraftingValues();
            setProgressSlider(physicalMachine.GetProgress());
        }
        else
        {
            craftingWindow.SetActive(false);
        }
        List<CraftingRecipe> recipesForMachine = Inventory.GetRecipesForMachine(machine);
        for (int i = 0; i < recipesForMachine.Count; i++)
        {
            CraftingRecipe recipe = recipesForMachine[i];
            GameObject t;
            t = Instantiate(recipePrefab, recipeList.transform);
            t.transform.GetChild(0).GetComponent<Image>().sprite = recipe.Icon;
            t.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = recipe.DisplayName;
            t.GetComponent<Button>().onClick.AddListener(() => RecipeClicked(recipe));
            //craftingWindow.GetComponent<Image>().sprite = machine.Icon;
            transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = machine.name;
            InitRecipeWindow(t, recipe);
        }
    }

    public void ChangeCraftAmount(int changeValue = 1)
    {
        craftingMachine.SetAmount(craftingMachine.GetAmount()+changeValue);
        SetCraftingValues();
        craftingMachine.SetIsCrafting(false);
    }

    void SetCraftAmount(int newValue)
    {
        int checkedValue = MakeSureAmountIsValid(newValue);
        craftingMachine.SetAmount(checkedValue);
        SetCraftingValues();
        craftingMachine.SetIsCrafting(false);
    }

    int MakeSureAmountIsValid(int amount)
    {
        return Mathf.Clamp(amount, 1, Inventory.MaxAmountCraftable(recipeMarked));
    }

    public void SetCraftingValues(InteractableCraftingMachine machineTryingToSet)
    {
        if(machineTryingToSet == craftingMachine)
        {
            SetCraftingValues();
        }
    }

    void UpdateAmountSliderValues()
    {
        if (recipeMarked != null)
        {
            amountSlider.maxValue = Inventory.MaxAmountCraftable(recipeMarked);
        }
    }

    public void OnAmountSliderValueChange()
    {
        if (amountSlider.value != craftingMachine.GetAmount())
        {
            SetCraftAmount((int)amountSlider.value);
        }
    }

    void SetCraftingValues()
    {
        int amount = craftingMachine.GetAmount();
        if(amount < 2)
        {
            UIManager.SetButtonIsEnabled(lowerAmountBtn, false);
        }
        else
        {
            UIManager.SetButtonIsEnabled(lowerAmountBtn, true);
        }

        if(!Inventory.IsCraftable(recipeMarked, amount + 1))
        {
            UIManager.SetButtonIsEnabled(increaseAmountBtn, false);
        }
        else
        {
            UIManager.SetButtonIsEnabled(increaseAmountBtn, true);
        }
        amountTxt.GetComponent<TextMeshProUGUI>().text = amount.ToString();
        amountSlider.value = amount;
        setProgressSlider(craftingMachine.GetProgress());
    }

    void setProgressSlider(float value)
    {
        progressSlider.value = value;
    }

    public void CancelCraft()
    {
        if(craftingMachine != null)
        {
            craftingMachine.GetComponent<InteractableCraftingMachine>().CancelCraft();
            craftingWindow.SetActive(false);
        }
    }

    void InitRecipeWindow(GameObject window, CraftingRecipe recipe)
    {
        GameObject ingredientList = window.transform.GetChild(2).gameObject; //super duper scary sätt att fixa på
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
        if(Inventory.IsCraftable(recipe)) //den här if-satsen ska kanske inte vara här
        {
            recipeMarked = recipe;
            craftingWindow.SetActive(true);
            recipeName.GetComponent<TextMeshProUGUI>().text = recipe.DisplayName;
            craftingMachine.GetComponent<InteractableCraftingMachine>().CancelCraft();
            //lite fusklösning, men tror det fungerar i alla situationer?
            amountSlider.value = 1;
            SetCraftingValues();
            UpdateAmountSliderValues();
            //Den här ska kanske vara recipe.icon istället, men nu måste receptet i sig inte ha en ikon
            recipeImg.GetComponent<Image>().sprite = recipe.itemCrafted.Icon;
        }
    }

    public void CraftItem()
    {
        if(Inventory.IsCraftable(recipeMarked)) //kanske onödig, men känns bra att dubbelkolla
        {
            craftingMachine.GetComponent<InteractableCraftingMachine>().CraftItems(recipeMarked, characterWhoOpenedWindow);
        }
    }
}
