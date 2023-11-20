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
        }
        //selectedCharacter.gameObject.GetComponent<Exploration>().Explore();

        /*if (Input.GetKeyDown("space") && selectedCharacter != null)
        {
            //StartCoroutine(ExploringProcess());
            selectedCharacter.gameObject.GetComponent<Exploration>().Explore();
        }*/
    }

    [SerializeField]
    GameObject characterStatsWindow;
    static GameObject characterStatsWindowStatic;
    void UpdateCharacterStatsUI()
    {
        characterStatsWindowStatic.GetComponent<CharacterStatsHandler>().UpdateHealth(selectedCharacter.health);
        characterStatsWindowStatic.GetComponent<CharacterStatsHandler>().UpdateHunger(selectedCharacter.hunger);
    }

    public void FeedCharacter(Food food)
    {
        if (selectedCharacter != null)
        {
            selectedCharacter.ConsumeFood(food);
        }
    }

    private void Start()
    {
        unSelectedModifier = unSelectedModifierSetter;
        selectedModifier = selectedModifierSetter;
        movementSpeed = movementSpeedSetter;
        Character.onTaskCompletion += TaskCompleted;

        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();



        //temp
        characterStatsWindow.GetComponent<CharacterStatsHandler>().SetUp();
        characterStatsWindow.SetActive(false);
        characterStatsWindowStatic = characterStatsWindow;
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
                ChestContent chest = selectedCharacter.itemInteractedWith.GetComponent<ChestContent>();
                if (chest == null)
                {
                    chest = selectedCharacter.itemInteractedWith.gameObject.AddComponent<ChestContent>();
                }
                chest.CheckContent();
                break;
            case CharacterTasks.exploring:
                TextLog.AddLog(selectedCharacter.name + " went exploring.");
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
            characterStatsWindowStatic.SetActive(true);
        }
    }

    public static void setCharacterVisual(Character character, bool isSelected)
    {
        if(isSelected)
        {
            character.GetComponent<MeshRenderer>().material = selectedModifier.material;
        }
        else
        {
            character.GetComponent<MeshRenderer>().material = unSelectedModifier.material;
        }
    }

    public Character GetSelectedCharacter() 
    {
        return selectedCharacter;
    }
}
