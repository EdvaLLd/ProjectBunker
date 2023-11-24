using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public class SelectionVisibilityModifier
{
    public Material material;
}

public enum CharacterTasks
{
    none, crafting, inspecting, looting, exploring
}

public class UnitController : MonoBehaviour
{
    //Hur man ser skillnad om en karaktär är markerad eller inte
    [SerializeField]
    SelectionVisibilityModifier unSelectedModifierSetter, selectedModifierSetter;

    //Ska kanske finnas en speedmodifier på varje karaktär?
    [SerializeField]
    float movementSpeedSetter = 1;

    UIManager uiManager;


    //Lägg till alla serializedfield-variabler här som static och i start
    static SelectionVisibilityModifier unSelectedModifier, selectedModifier;
    public static float movementSpeed; 


    static Character selectedCharacter = null;


    //(denna är nog anpassad för procent, så viktigt att variablerna går mellan 0 och 100)
    //avgör hur ofta UIn uppdateras, 5 = var femte procent
    int howOftenToUpdateStats = 5;


    static GameObject characterStatsWindowStatic;


    [SerializeField]
    CraftingRecipe[] recipes;

    private void Start()
    {
        unSelectedModifier = unSelectedModifierSetter;
        selectedModifier = selectedModifierSetter;
        movementSpeed = movementSpeedSetter;
        Character.onTaskCompletion += TaskCompleted;

        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();



        characterStatsWindowStatic = GameObject.FindGameObjectWithTag("CharacterStatsWindow");
        characterStatsWindowStatic.SetActive(false);

        //Det här är temp och ska tas bort när man kan få recept på bättre sätt
        for (int i = 0; i < recipes.Length; i++)
        {
            Inventory.AddRecipeToMachines(recipes[i]);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Inventory.AddItem(Database.GetItemWithID("01001"));
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Inventory.AddItem(Database.GetItemWithID("04001"));
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (selectedCharacter != null)
            {
                selectedCharacter.ConsumeFood(Database.GetItemWithID("04001") as Food);
            }
        }
        if (selectedCharacter != null)
        {
            UpdateCharacterStatsUI();
            if (Input.GetMouseButtonDown(1))
            {
                selectedCharacter.MoveToPos(HelperMethods.CursorToWorldCoord());
            }
        }
    }
    void UpdateCharacterStatsUI()
    {
        characterStatsWindowStatic.GetComponent<CharacterStatsHandler>().UpdateHealth(((int)(selectedCharacter.health / howOftenToUpdateStats) + 1) * howOftenToUpdateStats);
        characterStatsWindowStatic.GetComponent<CharacterStatsHandler>().UpdateHunger(((int)(selectedCharacter.hunger / howOftenToUpdateStats) + 1) * howOftenToUpdateStats);
    }

    public void FeedCharacter(Food food)
    {
        if (selectedCharacter != null)
        {
            selectedCharacter.ConsumeFood(food);
        }
    }

    public void TaskCompleted(Character character)
    {
        switch (character.task)
        {
            case CharacterTasks.none:
                print("shouldnt be here");
                break;
            case CharacterTasks.crafting:
                uiManager.ActivateWindow(uiManager.craftingWindow);
                if (uiManager.craftingWindow.active)
                {
                    uiManager.craftingWindow.GetComponent<CraftingWindow>().InitCraftingWindow(character.item as CraftingMachine);
                }
                break;
            case CharacterTasks.inspecting:
                TextLog.AddLog($"Inspected item: {character.item.Description}");
                break;
            case CharacterTasks.looting:
                ChestContent chest = character.itemInteractedWith.GetComponent<ChestContent>();
                if (chest == null)
                {
                    TextLog.AddLog("Chest is empty");
                }
                else
                {
                    TextLog.AddLog("Interacted with item " + character.itemInteractedWith.item.DisplayName);
                    chest.CheckContent();
                }
                break;
            case CharacterTasks.exploring:
                selectedCharacter.gameObject.GetComponent<Exploration>().Explore();
                UnitController.SwapSelectedCharacter(null);
                break;

            default:
                break;
        }
    }


    public static void InteractedWith(CharacterTasks task, ItemBase item, InteractableItem itemInteractedWith)
    {
        if(selectedCharacter != null)
        {
            selectedCharacter.task = task;
            selectedCharacter.item = item;
            selectedCharacter.InteractedWithItem(itemInteractedWith);
        }
    }


    public static void SwapSelectedCharacter(Character newSelectedCharacter)
    {
        if (selectedCharacter == newSelectedCharacter)
        {
            setCharacterVisual(selectedCharacter, false);
            selectedCharacter = null;
            characterStatsWindowStatic.SetActive(false);
        }
        else
        {
            if (selectedCharacter != null)
            {
                setCharacterVisual(selectedCharacter, false);
            }
            selectedCharacter = newSelectedCharacter;
            setCharacterVisual(selectedCharacter, true);
            characterStatsWindowStatic.GetComponent<CharacterStatsHandler>().SetUp(selectedCharacter);
            characterStatsWindowStatic.SetActive(true);
        }
    }

    public static void setCharacterVisual(Character character, bool isSelected)
    {
        if(isSelected)
        {
            //character.transform.GetChild(1).GetComponent<MeshRenderer>().material = selectedModifier.material;
        }
        else
        {
            //character.transform.GetChild(1).GetComponent<MeshRenderer>().material = unSelectedModifier.material;
        }
        character.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = isSelected;
    }

    public Character GetSelectedCharacter()
    {
        return selectedCharacter;
    }
}
