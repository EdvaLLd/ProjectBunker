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
    //Hur man ser skillnad om en karakt�r �r markerad eller inte
    [SerializeField]
    SelectionVisibilityModifier unSelectedModifierSetter, selectedModifierSetter;

    //Ska kanske finnas en speedmodifier p� varje karakt�r?
    [SerializeField]
    float movementSpeedSetter = 1;

    UIManager uiManager;


    //L�gg till alla serializedfield-variabler h�r som static och i start
    static SelectionVisibilityModifier unSelectedModifier, selectedModifier;
    public static float movementSpeed; 


    static Character selectedCharacter = null;


    //(denna �r nog anpassad f�r procent, s� viktigt att variablerna g�r mellan 0 och 100)
    //avg�r hur ofta UIn uppdateras, 5 = var femte procent
    int howOftenToUpdateStats = 5;


    static GameObject characterStatsWindowStatic;


    [SerializeField]
    CraftingRecipe[] recipes;
    [SerializeField]
    RecipeSlot[] items;

    private void Awake()
    {
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        characterStatsWindowStatic = GameObject.FindGameObjectWithTag("CharacterStatsWindow");
    }

    private void Start()
    {
        unSelectedModifier = unSelectedModifierSetter;
        selectedModifier = selectedModifierSetter;
        movementSpeed = movementSpeedSetter;
        Character.onTaskCompletion += TaskCompleted;




        characterStatsWindowStatic.SetActive(false);

        //Det h�r �r temp och ska tas bort n�r man kan f� recept och items p� b�ttre s�tt
        for (int i = 0; i < recipes.Length; i++)
        {
            Inventory.AddRecipeToMachines(recipes[i]);
        }
        for (int i = 0; i < items.Length; i++)
        {
            Inventory.AddItem(items[i].item, items[i].amount);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Inventory.AddItem(Database.GetItemWithID("04001")); //br�d
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Inventory.AddItem(Database.GetItemWithID("01010")); //water
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Inventory.AddItem(Database.GetItemWithID("01004")); //electric cable
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Inventory.AddItem(Database.GetItemWithID("01001")); //wood
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Inventory.AddItem(Database.GetItemWithID("01008")); //metal
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Inventory.AddItem(Database.GetItemWithID("01002")); //leather
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
                /*uiManager.ActivateWindow(uiManager.craftingWindow);
                if (uiManager.craftingWindow.active)
                {
                    uiManager.craftingWindow.GetComponent<CraftingWindow>().InitCraftingWindow(character.item as CraftingMachine);
                }*/
                character.itemInteractedWith.GetComponent<InteractableCraftingMachine>().InteractedWith(character);
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
                characterStatsWindowStatic.SetActive(false);
                SwapSelectedCharacter(selectedCharacter);

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
            //characterStatsWindowStatic.SetActive(false);
            characterStatsWindowStatic.GetComponent<Animator>().SetTrigger("SlideDownTrigger");
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
            characterStatsWindowStatic.GetComponent<Animator>().SetTrigger("SlideUpTrigger");
        }
    }

    public static void setCharacterVisual(Character character, bool isSelected)
    {
        /*if(isSelected)
        {
            //character.transform.GetChild(1).GetComponent<MeshRenderer>().material = selectedModifier.material;
        }
        else
        {
            //character.transform.GetChild(1).GetComponent<MeshRenderer>().material = unSelectedModifier.material;
        }*/
        character.transform.GetChild(1).gameObject.SetActive(isSelected);
    }

    public static Character GetSelectedCharacter()
    {
        return selectedCharacter;
    }
}
