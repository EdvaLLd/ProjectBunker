using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCraftingMachine : InteractableItem
{
    CraftingRecipe currentRecipeBeingCrafted;
    int amountLeft = 1;
    float progress; //0-1
    Character characterOnStation;
    bool isCrafting = false;

    float DEBUGCRAFTINGTIME = 5;

    CraftingWindow craftingWindow;

    private void Awake()
    {
        craftingWindow = GameObject.FindGameObjectWithTag("CraftingWindow").GetComponent<CraftingWindow>();
    }

    public void InteractedWith(Character character)
    {
        UIManager.ActivateWindow(UIManager.craftingWindow);
        UIManager.craftingWindow.GetComponent<CraftingWindow>().InitCraftingWindow(character.item as CraftingMachine, this, character);
        //characterOnStation = character;
    }
    public void CraftItems(CraftingRecipe recipe, Character characterCrafting)
    {
        isCrafting = true;
        characterOnStation = characterCrafting;
        if (recipe != currentRecipeBeingCrafted)
        {
            currentRecipeBeingCrafted = recipe;
            RemoveItemsRequiredForCraft();
        }
    }
    public void CharacterLeftStation(Character character)
    {
        if (character == characterOnStation)
        {
            characterOnStation = null;
            isCrafting = false;
        }
    }

    public void SetAmount(int value)
    {
        amountLeft = value;
    }

    public int GetAmount()
    {
        return amountLeft;
    }

    public bool GetIsCrafting()
    {
        return isCrafting;
    }

    public void SetIsCrafting(bool value)
    {
        isCrafting = value;
    }

    public CraftingRecipe GetRecipe()
    {
        return currentRecipeBeingCrafted;
    }

    public float GetProgress()
    {
        return progress;
    }

    public void CancelCraft()
    {
        progress = 0;
        //varför fuckar den här upp om den ligger under rad 82??????
        isCrafting = false;
        RefundItemsForCraft();
        amountLeft = 1;
        currentRecipeBeingCrafted=null;
    }

    bool RemoveItemsRequiredForCraft()
    {
        if (currentRecipeBeingCrafted != null)
        {
            if (Inventory.IsCraftable(currentRecipeBeingCrafted, amountLeft))
            {
                for (int i = 0; i < currentRecipeBeingCrafted.Ingredients.Count; i++)
                {
                    Inventory.RemoveItem(currentRecipeBeingCrafted.Ingredients[i].item, currentRecipeBeingCrafted.Ingredients[i].amount * amountLeft);
                }
                return true;
            }
        }
        return false;
    }

    void RefundItemsForCraft()
    {
        if (currentRecipeBeingCrafted != null)
        {
            for (int i = 0; i < currentRecipeBeingCrafted.Ingredients.Count; i++)
            {
                Inventory.AddItem(currentRecipeBeingCrafted.Ingredients[i].item, currentRecipeBeingCrafted.Ingredients[i].amount * amountLeft);
            }
        }
    }

    private void Update()
    {
        if(isCrafting)
        {
            progress += Time.deltaTime / DEBUGCRAFTINGTIME;
            if(progress > 1)
            {
                amountLeft--;
                progress = 0;
                Inventory.AddItem(currentRecipeBeingCrafted.itemCrafted, currentRecipeBeingCrafted.itemAmount);

                if (amountLeft < 1)
                {
                    isCrafting = false;
                    craftingWindow.FinishedCrafting(this);
                    currentRecipeBeingCrafted = null;
                    amountLeft = 1;
                    //Kan även fixa med animationer och typ uigrejer här
                }
                /*else
                {
                    if (!Inventory.IsCraftable(currentRecipeBeingCrafted))
                    {
                        //varna UI-mässigt att det sket sig
                        CharacterLeftStation(characterOnStation);
                        isCrafting = false;
                    }
                    else
                    {
                        RemoveItemsRequiredForCraft();
                    }
                }*/
            }
            craftingWindow.SetCraftingValues(this);
        }
    }
}
