using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCraftingMachine : InteractableItem
{
    CraftingRecipe currentRecipeBeingCrafted;
    int amountLeft = 1;
    int amountPayedFor = 0;
    float progress; //0-1
    Character characterOnStation;
    bool isCrafting = false;

    CraftingWindow craftingWindow;

    private void Awake()
    {
        craftingWindow = GameObject.FindGameObjectWithTag("CraftingWindow").GetComponent<CraftingWindow>();
    }

    public void InteractedWith(Character character)
    {
        UIManager.SetWindowActive(UIManager.craftingWindow);
        UIManager.craftingWindow.GetComponent<CraftingWindow>().InitCraftingWindow(character.item as CraftingMachine, this, character);
        //characterOnStation = character;
    }
    public void CraftItems(CraftingRecipe recipe, Character characterCrafting)
    {
        SetIsCrafting(true);
        characterOnStation = characterCrafting;
        if (recipe != currentRecipeBeingCrafted)
        {
            currentRecipeBeingCrafted = recipe;
            RemoveItemsRequiredForCraft();
        }
        else
        {
            if(amountPayedFor < amountLeft)
            {
                RemoveItemsRequiredForCraft();
            }
            else if(amountPayedFor > amountLeft)
            {
                RefundItemsForCraft(amountPayedFor - amountLeft);
            }
        }
    }
    public void CharacterLeftStation(Character character)
    {
        if (character == characterOnStation)
        {
            characterOnStation = null;
            SetIsCrafting(false);
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

    public int GetPayedAmount()
    {
        return amountPayedFor;
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
        //varf�r fuckar den h�r upp om den ligger under rad 82??????
        SetIsCrafting(false);
        RefundItemsForCraft();
        amountLeft = 1;
        amountPayedFor = 0;
        currentRecipeBeingCrafted=null;
    }

    bool RemoveItemsRequiredForCraft()
    {
        if (currentRecipeBeingCrafted != null)
        {
            if (Inventory.IsCraftable(currentRecipeBeingCrafted, amountLeft - amountPayedFor))
            {
                for (int i = 0; i < currentRecipeBeingCrafted.Ingredients.Count; i++)
                {
                    Inventory.RemoveItem(currentRecipeBeingCrafted.Ingredients[i].item, currentRecipeBeingCrafted.Ingredients[i].amount * (amountLeft-amountPayedFor));
                }
                amountPayedFor = amountLeft;
                return true;
            }
        }
        return false;
    }

    void RefundItemsForCraft()
    {
        RefundItemsForCraft(amountPayedFor);
    }

    void RefundItemsForCraft(int amountToRefund)
    {
        if (currentRecipeBeingCrafted != null)
        {
            for (int i = 0; i < currentRecipeBeingCrafted.Ingredients.Count; i++)
            {
                Inventory.AddItem(currentRecipeBeingCrafted.Ingredients[i].item, currentRecipeBeingCrafted.Ingredients[i].amount * amountToRefund);
            }
            amountPayedFor -= amountToRefund;
        }
    }

    private void Update()
    {
        if(isCrafting)
        {
            progress += Time.deltaTime / currentRecipeBeingCrafted.craftingTime;
            if(progress > 1)
            {
                amountLeft--;
                amountPayedFor--;
                progress = 0;
                Inventory.AddItem(currentRecipeBeingCrafted.itemCrafted, currentRecipeBeingCrafted.itemAmount);

                if (amountPayedFor < 1)
                {
                    SetIsCrafting(false);
                    craftingWindow.FinishedCrafting(this);
                    currentRecipeBeingCrafted = null;
                    amountLeft = 1;
                    //Kan �ven fixa med animationer och typ uigrejer h�r
                }
                /*else
                {
                    if (!Inventory.IsCraftable(currentRecipeBeingCrafted))
                    {
                        //varna UI-m�ssigt att det sket sig
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
