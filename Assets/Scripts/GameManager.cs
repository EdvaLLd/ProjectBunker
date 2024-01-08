using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IDataPersistance
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }
    #region Variables
    #region Miscalanious.
    /*public enum difficulties { Easy, Normal, Hard }; // Was just for practice when I began using this Variable type again, at the start of our project.
    [SerializeField]
    private difficulties difficulty;*/

    public bool isFirstRun = true;
    private SkyboxController skyboxManager;
    #endregion
    #region Location
    private static Locations.Location[] explorableLocations = new Locations.Location[System.Enum.GetNames(typeof(Locations.Location.environments)).Length];


    [Header("Location")]
    //public Looting.LootItem looting = new Looting.LootItem();

    [Tooltip("0=Lake, 1=City, 2=Factory, 3=Forest"), NonReorderable]
    public Looting.LocationLootItems[] locationalLoot = new Looting.LocationLootItems[System.Enum.GetNames(typeof(Locations.Location.environments)).Length - 1];
    #endregion
    #region Diary
    [Header("Diary")]
    public static int leftPageIndex = 0;

    public Diary gameDiary = new Diary();
    #endregion
    #region Events
    [Header("Events")]
    public static int eventIndex = 0;

    public ExplorationBase.StandardExploreEvent[] standardExploreEvents;
    public ExplorationBase.ExploreEvent[] mainExploreEvents;
    public ExplorationBase.RandomExploreEvent[] randomExploreEvents;
    public ExplorationBase.LimitedExploreEvent[] limitedExploreEvents;
    #endregion
    #region Character
    [Header("Characters")]
    public string[] characterNames =
    {
        "Gonzalez",
        "John",
        "Emily",
        "Michael",
        "Sarah",
        "David",
        "Awe",
        "Jessica",
        "Matthew",
        "Amanda",
        "Christopher",
        "Elizabeth",
        "Emma",
        "Edvard",
        "Ella",
        "Cassandra",
        "Alexander",
        "Thomas",
        "Patricia",
    };
    [HideInInspector]
    public int availableIdKey = 1;
    public static JsonCharacterList serializedCharacterList = new JsonCharacterList();
    [SerializeField, Tooltip("This field contains the prefab base that will be used when loading serialized characters.")] 
    private GameObject characterPrefab;
    //public static CharacterList characterList = new CharacterList();
    #endregion
    #region Time
    private float hour = 6;
    private float minute;
    private int day;
    #endregion
    #endregion

    private void Awake()
    {
        instance = this;
        
        skyboxManager = GameObject.FindObjectOfType<SkyboxController>();

        explorableLocations = Locations.SetExplorableLocations();

        //this.gameDiary.UpdateDiaryGUI();

        serializedCharacterList.serializedCharacters = new List<JsonCharacter>();/*new Character[0];*/
    }

    private void Update()
    {
        skyboxManager.DayAndNightCycle(skyboxManager.cycleRate);
        DayAndTimeCounter();
    }

    private void DayAndTimeCounter()
    {
        SkyboxController skyboxManager = GameObject.FindObjectOfType<SkyboxController>();

        //SetMinute(skyboxManager);
        //SetHour(skyboxManager);

        minute += skyboxManager.cycleRate * 24 / 360 * 60 * Time.deltaTime;
        if (minute >= 60) { minute = 0; hour++; }

        //hour += skyboxManager.cycleRate / 60;
        if (hour >= 24) { hour = 0; day++; }

        string displayTime = Mathf.FloorToInt(hour).ToString("00") + ":" + Mathf.FloorToInt(minute).ToString("00");
        string dayCount = day + " days";

        GameObject.Find("DayTimeStamp").transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = displayTime;
        GameObject.Find("DayCounter").transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = dayCount;
    }

    public static Locations.Location[] GetExplorableLocations() 
    {
        return explorableLocations;
    }

    private List<JsonCharacter> SerializeSceneCharacters() 
    {
        List<JsonCharacter> returnList = new List<JsonCharacter>();

        Character[] characters = FindObjectsOfType<Character>();

        if (characters.Length > 0) 
        {
            for (int index = 0; index < characters.Length; index++)
            {
                returnList.Add(JsonCharacter.CharacterToJson(characters[index]));
            }
        }

        return returnList;
    }

    public void LoadData(GameData data)
    {


        #region Load variables

        #region Misc.
        isFirstRun = data.firstRun;
        #endregion

        #region Time
        hour = data.clockHour;
        minute = data.clockMinute;
        day = data.dayCount;
        #endregion

        #region Diary
        leftPageIndex = data.diaryLeftPageIndex;
        if (data.diary.entries.Count > 0) 
        {
            gameDiary.entries = data.diary.entries;
        }
        
        #endregion

        #region Event
        eventIndex = data.mainEventIndex;
        #endregion

        #region Character
        //characterArray = data.arrayOfCharacters;
        availableIdKey = data.availableIdKey;

        if (data.listOfCharacters.serializedCharacters.Count > 0)
        {
            serializedCharacterList.serializedCharacters = data.listOfCharacters.serializedCharacters;
        }
        else 
        {
            Debug.LogError("No characters serialized.");
        }
        
        if (isFirstRun) 
        {
            SpawnStartCharacter();
        }
        SpawnSerializedCharacters(serializedCharacterList.serializedCharacters);
        //characterArray.test = data.arrayOfCharacters.test;
        #endregion
        #endregion
    }

    public void SaveData(ref GameData data) 
    {
        #region Save variables

        #region Misc.
        data.firstRun = isFirstRun;
        #endregion

        #region Time
        data.clockHour = hour;
        data.clockMinute = minute;
        data.dayCount = day;
        #endregion

        #region Diary
        data.diaryLeftPageIndex = leftPageIndex;
        if (gameDiary.entries.Count > 0) 
        {
            data.diary.entries = gameDiary.entries;
        }
        #endregion

        #region Event
        data.mainEventIndex = eventIndex;
        #endregion

        #region Character
        data.availableIdKey = availableIdKey;

        serializedCharacterList.serializedCharacters = SerializeSceneCharacters();
        //data.arrayOfCharacters = characterArray;
        if (serializedCharacterList.serializedCharacters.Count > 0) 
        {
            data.listOfCharacters.serializedCharacters = serializedCharacterList.serializedCharacters;
        }
        //data.arrayOfCharacters.test = characterArray.test;
        #endregion
        #endregion
    }

    private void SpawnSerializedCharacters(List<JsonCharacter> loadedCharacters) 
    {
        if (loadedCharacters.Count <= 0) 
        {
            Debug.LogError("No characters serialized to spawn. \nSpawning of characters was canceled.");
            return;
        }
        if (characterPrefab == null) 
        {
            Debug.LogError("No prefab set for characterPrefab. \nSpawning of characters was canceled.");
            return;
        }
        if (!characterPrefab.TryGetComponent(out Character character))
        {
            Debug.LogError("No Character component present for prefab. Add Character script or change prefab to one with script present. \nSpawning of characters was canceled.");
            return;
        }

        for (int index = 0; index < loadedCharacters.Count; index++) 
        {
            bool alreadyExists = false;
            GameObject loadCharacter = new GameObject();
            loadCharacter = characterPrefab;

            JsonCharacter.JsonToCharacter(loadedCharacters[index], loadCharacter.GetComponent<Character>());

            for (int idCheckIndex = 0; idCheckIndex < GameObject.FindObjectsOfType<Character>().Length; idCheckIndex++) 
            {
                if (loadedCharacters[index].idKey == GameObject.FindObjectsOfType<Character>()[idCheckIndex].GetComponent<Character>().idKey) 
                {
                    OverwriteCharacter(loadCharacter.GetComponent<Character>(), GameObject.FindObjectsOfType<Character>()[idCheckIndex]);
                    //UpdateAvailableIdKey();
                    alreadyExists = true;
                    break;
                }

                if (idCheckIndex >= GameObject.FindObjectsOfType<Character>().Length) 
                {
                    break;
                }
            }

            if (!alreadyExists)
            {
                Instantiate(loadCharacter);
            }
            else 
            {
                Debug.Log("Character with idKey: " + loadedCharacters[index].idKey + " is already on the scene and will not be instantiated, instead it will be overwritten.");
            }
            UpdateAvailableIdKey();

            if (index >= loadedCharacters.Count) 
            {
                break;
            } 
        }
    }

    private void SpawnStartCharacter() 
    {
        if (characterPrefab == null)
        {
            Debug.LogError("No prefab set for characterPrefab. \nSpawning of character was canceled.");
            return;
        }

        GameObject character = Instantiate(characterPrefab);
        UpdateAvailableIdKey();

        character.transform.position = new Vector3(5.3f, 0.362f, 0.542f);
    }

    private void OverwriteCharacter(Character loadedCharacter, Character targetCharacter) 
    {
        targetCharacter.idKey = loadedCharacter.idKey;
        targetCharacter.hasName = loadedCharacter.hasName;
        targetCharacter.idIsSet = loadedCharacter.idIsSet;
        targetCharacter.loadedCharacterPosition = loadedCharacter.loadedCharacterPosition;
        targetCharacter.itemInteractedWithBoxCollider = loadedCharacter.itemInteractedWithBoxCollider;
        targetCharacter.path = loadedCharacter.path;
        targetCharacter.posMovingTo = loadedCharacter.posMovingTo;
        targetCharacter.move = loadedCharacter.move;
        targetCharacter.item = loadedCharacter.item;
        targetCharacter.task = loadedCharacter.task;
        targetCharacter.itemInteractedWith = loadedCharacter.itemInteractedWith;
        //SerializedCharacter.onTaskCompletion = characters[index]._; // Solve how to get this variable.
        //serializedCharacter.gearEquipped = sceneCharacter.gearEquipped;
        targetCharacter.hunger = loadedCharacter.hunger;
        targetCharacter.health = loadedCharacter.health;
        targetCharacter.isAlive = loadedCharacter.isAlive;
        targetCharacter.lowHealthWarningShowed = loadedCharacter.lowHealthWarningShowed;
        targetCharacter.notHungryTime = loadedCharacter.notHungryTime;
        targetCharacter.maxHunger = loadedCharacter.maxHunger;
        targetCharacter.maxHealth = loadedCharacter.maxHealth;
        targetCharacter.createNewPath = loadedCharacter.createNewPath;
        targetCharacter.newGoalPos = loadedCharacter.newGoalPos;
        targetCharacter.characterName = loadedCharacter.characterName;
        //serializedCharacter.deseases = sceneCharacter.deseases;
        //targetCharacter.characterAnim = inputCharacter.characterAnim;
        targetCharacter.statuses = loadedCharacter.statuses;
        targetCharacter.workMultiplier = loadedCharacter.workMultiplier;
        targetCharacter.marker = loadedCharacter.marker;
        targetCharacter.reasonsToWarn = loadedCharacter.reasonsToWarn;
        targetCharacter.audioClip = loadedCharacter.audioClip;
        targetCharacter.audioSource = loadedCharacter.audioSource;
        
        targetCharacter.SetLoadedPosition();
    }

    private void OnApplicationQuit()
    {
        if (isFirstRun) 
        {
            isFirstRun = false;
        }
    }

    public void UpdateAvailableIdKey() 
    {
        Character[] characters = GameObject.FindObjectsOfType<Character>();
        int[] idKeys = new int[characters.Length];

        availableIdKey = Mathf.Max(idKeys) + 1;
    }
}



[System.Serializable]
public class JsonCharacterList
{
    public List<JsonCharacter> serializedCharacters;
}

[System.Serializable]
public class CharacterList
{
    public List<Character> characters;
}