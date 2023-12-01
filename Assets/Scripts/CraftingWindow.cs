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
        print("New recipe! " + recipe.DisplayName);
        if(craftingMachine != null && Inventory.IsRecipeCraftableInMachine(craftingMachine.item as CraftingMachine, recipe))
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
        if(physicalMachine.GetProgress() != 0) //kommer inte det h�r g�ra s� om man timear och avbryter tasken n�r timern �r p� EXAKT 0 s� resettas den??
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
        if (recipeMarked != null && craftingMachine != null && !craftingMachine.GetIsCrafting())
        {
            amountSlider.maxValue = Inventory.MaxAmountCraftable(recipeMarked);
            SetCraftingValues();
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
        if (!craftingMachine.GetIsCrafting())
        {
            if (amount < 2)
            {
                UIManager.SetButtonIsEnabled(lowerAmountBtn, false);
            }
            else
            {
                UIManager.SetButtonIsEnabled(lowerAmountBtn, true);
            }

            if (!Inventory.IsCraftable(recipeMarked, amount + 1))
            {
                UIManager.SetButtonIsEnabled(increaseAmountBtn, false);
            }
            else
            {
                UIManager.SetButtonIsEnabled(increaseAmountBtn, true);
            }
            amountSlider.value = amount;
        }
        setProgressSlider(craftingMachine.GetProgress());
        amountTxt.GetComponent<TextMeshProUGUI>().text = amount.ToString();
    }

    void setProgressSlider(float value)
    {
        progressSlider.value = value;
    }

    public void CancelCraft()
    {
        if(craftingMachine != null)
        {
            if (!craftingMachine.GetIsCrafting()) craftingWindow.SetActive(false);
            else
            {
                craftingMachine.CancelCraft();
                amountSlider.enabled = true;
                SetCraftingValues();
            }
        }
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
        if(Inventory.IsCraftable(recipe)) //den h�r if-satsen ska kanske inte vara h�r
        {
            recipeMarked = recipe;
            craftingWindow.SetActive(true);
            recipeName.GetComponent<TextMeshProUGUI>().text = recipe.DisplayName;
            craftingMachine.GetComponent<InteractableCraftingMachine>().CancelCraft();
            //lite fuskl�sning, men tror det fungerar i alla situationer?
            amountSlider.value = 1;
            SetCraftingValues();
            UpdateAmountSliderValues();
            //Den h�r ska kanske vara recipe.icon ist�llet, men nu m�ste receptet i sig inte ha en ikon
            recipeImg.GetComponent<Image>().sprite = recipe.itemCrafted.Icon;
        }
    }

    public void CraftItem()
    {
        if(Inventory.IsCraftable(recipeMarked, craftingMachine.GetAmount())) //kanske on�dig, men k�nns bra att dubbelkolla
        {
            UIManager.SetButtonIsEnabled(lowerAmountBtn, false);
            UIManager.SetButtonIsEnabled(increaseAmountBtn, false);
            amountSlider.enabled = false;
            craftingMachine.GetComponent<InteractableCraftingMachine>().CraftItems(recipeMarked, characterWhoOpenedWindow);
        }
    }

    public void FinishedCrafting(InteractableCraftingMachine machine)
    {
        if(machine == craftingMachine)
        {
            if(Inventory.MaxAmountCraftable(craftingMachine.GetRecipe()) == 0)
            {
                craftingWindow.SetActive(false);
            }
        }
    }
}
