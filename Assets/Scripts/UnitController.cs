using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;

[Serializable]
public class SelectionVisibilityModifier
{
    public Material material;
}

public enum CharacterTasks
{
    none, crafting, inspecting, looting, exploring, eating
}

public class UnitController : MonoBehaviour
{
    //Hur man ser skillnad om en karakt�r �r markerad eller inte
    [SerializeField]
    SelectionVisibilityModifier unSelectedModifierSetter, selectedModifierSetter;

    //Ska kanske finnas en speedmodifier p� varje karakt�r?
    [SerializeField]
    float movementSpeedSetter = 1;


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

    static List<Character> allCharacters = new List<Character>();


    //namn på karaktärerna
    static TextMeshProUGUI characterName;

    static GameObject statusHolderGO;


    //Speltesttimer
    float timerForPlaytest = 30;
    bool randomDesease = false;

    private void Awake()
    {
        characterStatsWindowStatic = GameObject.FindGameObjectWithTag("CharacterStatsWindow");
        characterName = GameObject.FindGameObjectWithTag("CharacterName").GetComponent<TextMeshProUGUI>();
        statusHolderGO = GameObject.FindGameObjectWithTag("StatusHolder");
    }

    private void Start()
    {
        unSelectedModifier = unSelectedModifierSetter;
        selectedModifier = selectedModifierSetter;
        movementSpeed = movementSpeedSetter;
        Character.onTaskCompletion += TaskCompleted;


        GameObject[] c = GameObject.FindGameObjectsWithTag("Character");
        foreach (GameObject ch in c)
        {
            Character character;
            if(ch.TryGetComponent(out character))
            {
                allCharacters.Add(character);
            }
        }

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
        if (!randomDesease)
        {
            timerForPlaytest -= Time.deltaTime;
            if (timerForPlaytest < 0)
            {
                allCharacters[UnityEngine.Random.Range(0, allCharacters.Count - 1)].AddDesease<Flu>();
                randomDesease = true;
            }
        }

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
            if (Input.GetKeyDown(KeyCode.J))
            {
                selectedCharacter.AddDesease<Flu>();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                selectedCharacter.ConsumeFood(Database.GetItemWithID("04001") as Food);
            }
            UpdateCharacterStatsUI();
            if (Input.GetMouseButtonDown(1))
            {
                selectedCharacter.MoveToPos(HelperMethods.CursorToWorldCoord());
            }
        }
    }

    public static void AddCharacter(Character c)
    {
        allCharacters.Add(c);
    }

    public static void RemoveCharacter(Character c)
    {
        if(allCharacters.Contains(c)) allCharacters.Remove(c);
    }

    public static List<Character> GetCharacters()
    {
        return allCharacters;
    }
    void UpdateCharacterStatsUI()
    {
        characterStatsWindowStatic.GetComponent<CharacterStatsHandler>().UpdateHealth(((int)(selectedCharacter.health / howOftenToUpdateStats) + 1) * howOftenToUpdateStats);
        characterStatsWindowStatic.GetComponent<CharacterStatsHandler>().UpdateHunger(((int)(selectedCharacter.hunger / howOftenToUpdateStats) + 1) * howOftenToUpdateStats);
    }

    public static void FeedCharacter(Food food, Character character = null)
    {
        if (character == null)
        {
            if (selectedCharacter != null)
            {
                selectedCharacter.ConsumeFood(food);
            }
        }
        else
        {
            character.ConsumeFood(food);
        }
    }

    public void UseMedicine()
    {
        if(selectedCharacter != null)
        {
            selectedCharacter.TreatDesease();
        }
    }

    public void UseBandaid()
    {
        if (selectedCharacter != null)
        {
            //implementera när skador finns
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
                character.gameObject.GetComponent<Exploration>().Explore();
                //characterStatsWindowStatic.SetActive(false);
                if(character == selectedCharacter) SwapSelectedCharacter(selectedCharacter);

                break;
            case CharacterTasks.eating:
                character.itemInteractedWith.GetComponent<DiningArea>().OpenUI(character);

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
            characterName.text = selectedCharacter.name;
            SetCharacterStatusVisuals(selectedCharacter);
        }
    }

    public static void SetCharacterStatusVisuals(Character c)
    {
        if (c == selectedCharacter)
        {
            for (int i = 0; i < statusHolderGO.transform.childCount; i++)
            {
                if (selectedCharacter.HasStatus(statusHolderGO.transform.GetChild(i).GetComponent<StatusType>().status))
                {
                    statusHolderGO.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    statusHolderGO.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
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
