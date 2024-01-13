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
    CraftingMachine machine;

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
        Inventory.onInventoryUpdate += UpdateWindow;
    }

    private void Update()
    {
        if(characterWhoOpenedWindow != null && (characterWhoOpenedWindow.move || UnitController.GetSelectedCharacter() != characterWhoOpenedWindow))
        {
            UIManager.CloseWindow(gameObject);
        }
    }

    void NewRecipeAdded(CraftingRecipe recipe)
    {
        print("New recipe! " + recipe.itemCrafted.DisplayName);
        if(craftingMachine != null && Inventory.IsRecipeCraftableInMachine(craftingMachine.item as CraftingMachine, recipe))
        {
            print("Affects current machine");
            InitCraftingWindow(craftingMachine.item as CraftingMachine, craftingMachine, characterWhoOpenedWindow);
        }
    }

    void UpdateWindow()
    {
        if(UIManager.craftingWindow.active && craftingMachine != null)
        {
            InitCraftingWindow(machine, craftingMachine, characterWhoOpenedWindow);
        }
    }

    public void InitCraftingWindow(CraftingMachine machine, InteractableCraftingMachine physicalMachine, Character character)
    {
        this.machine = machine;
        craftingMachine = physicalMachine;
        characterWhoOpenedWindow = character;
        //Time.timeScale = 0;
        ClearChilds(recipeList.transform);
        if(physicalMachine.GetProgress() != 0) //kommer inte det här göra så om man timear och avbryter tasken när timern är på EXAKT 0 så resettas den??
        {
            print(physicalMachine.item.DisplayName + " " + physicalMachine.GetRecipe().itemCrafted.DisplayName);
            RecipeClicked(physicalMachine.GetRecipe());
            UpdateAmountSliderValues();
            //SetCraftingValues();
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
            t.transform.GetChild(0).GetComponent<Image>().sprite = recipe.itemCrafted.Icon;
            t.transform.GetChild(0).GetComponent<ItemHoverDesc>().item = recipe.itemCrafted;
            t.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = recipe.itemCrafted.DisplayName + " .";
            int timeConvertedSec = recipe.craftingTime % 60;
            int timeConvertedMin = recipe.craftingTime / 60;
            string finalTime = "";
            if (timeConvertedMin > 0) finalTime = timeConvertedMin.ToString() + " min ";
            if (timeConvertedSec > 0) finalTime += timeConvertedSec.ToString() + " s";
            if (!Inventory.IsCraftable(recipe)) t.GetComponent<Button>().interactable = false;
            else
            {
                t.transform.SetAsFirstSibling();
            }
            t.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = finalTime;
            t.GetComponent<Button>().onClick.AddListener(() => RecipeClicked(recipe));
            //craftingWindow.GetComponent<Image>().sprite = machine.Icon;
            InitRecipeWindow(t, recipe);
        }
        transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = machine.name;
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
        return Mathf.Clamp(amount, 1, Inventory.MaxAmountCraftable(recipeMarked) + craftingMachine.GetPayedAmount());
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
            amountSlider.maxValue = Inventory.MaxAmountCraftable(recipeMarked) + craftingMachine.GetPayedAmount();
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

            if (Inventory.MaxAmountCraftable(recipeMarked) + craftingMachine.GetPayedAmount() < amount + 1)
            {
                UIManager.SetButtonIsEnabled(increaseAmountBtn, false);
            }
            else
            {
                UIManager.SetButtonIsEnabled(increaseAmountBtn, true);
            }
            amountSlider.value = amount;
            amountSlider.enabled = true;
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
            if (!craftingMachine.GetIsCrafting())
            {
                craftingMachine.CancelCraft();
                craftingWindow.SetActive(false);
            }
            else
            {
                craftingMachine.SetIsCrafting(false);
                UpdateAmountSliderValues();
                SetCraftingValues();
            }
        }
    }

    void InitRecipeWindow(GameObject window, CraftingRecipe recipe)
    {
        GameObject ingredientList = window.transform.GetChild(3).gameObject; //super duper scary sätt att fixa på
        for (int i = 0; i < recipe.Ingredients.Count; i++)
        {
            GameObject t = UIManager.InitInventorySlot(recipe.Ingredients[i].item, recipe.Ingredients[i].amount, ingredientList.transform);
            if (Inventory.GetAmountOfItem(recipe.Ingredients[i].item) < recipe.Ingredients[i].amount)
            {
                t.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            else
            {
                t.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.green;
            }
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
        if (recipe != craftingMachine.GetRecipe())
        {
            craftingMachine.CancelCraft();
            if (Inventory.IsCraftable(recipe)) //den här if-satsen ska kanske inte vara här
            {
                recipeMarked = recipe;
                UIManager.SetWindowActive(craftingWindow);
                recipeName.GetComponent<TextMeshProUGUI>().text = recipe.itemCrafted.DisplayName;
                craftingMachine.CancelCraft();
                //lite fusklösning, men tror det fungerar i alla situationer?
                amountSlider.value = 1;
                SetCraftingValues();
                UpdateAmountSliderValues();
                //Den här ska kanske vara recipe.icon istället, men nu måste receptet i sig inte ha en ikon
                recipeImg.GetComponent<Image>().sprite = recipe.itemCrafted.Icon;
            }
            else
            {
                craftingWindow.SetActive(false);
            }
        }
        else
        {
            recipeMarked = craftingMachine.GetRecipe();
            UIManager.SetWindowActive(craftingWindow);
            recipeName.GetComponent<TextMeshProUGUI>().text = recipe.itemCrafted.DisplayName;
            SetCraftingValues();
            recipeImg.GetComponent<Image>().sprite = recipe.itemCrafted.Icon;
        }
    }

    public void CraftItem()
    {
        UIManager.SetButtonIsEnabled(lowerAmountBtn, false);
        UIManager.SetButtonIsEnabled(increaseAmountBtn, false);
        amountSlider.enabled = false;
        craftingMachine.GetComponent<InteractableCraftingMachine>().CraftItems(recipeMarked, characterWhoOpenedWindow);
    }

    public void FinishedCrafting(InteractableCraftingMachine machine)
    {
        if(machine == craftingMachine)
        {
            if(Inventory.MaxAmountCraftable(craftingMachine.GetRecipe()) == 0)
            {
                craftingWindow.SetActive(false);
            }
            else
            {
                UpdateAmountSliderValues();
                SetCraftingValues();
            }
        }
    }
}
