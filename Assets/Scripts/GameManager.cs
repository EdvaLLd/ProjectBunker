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
    public enum difficulties { Easy, Normal, Hard };
    [SerializeField]
    private difficulties difficulty;

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
    public static JsonCharacterList serializedCharacterList = new JsonCharacterList();
    public static CharacterList characterList = new CharacterList();
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

        //serializedCharacterList.serializedCharacters = new List<JsonCharacter>();/*new Character[0];*/
    }

    private void Start()
    {
        //characterList.sceneCharacters = PopulateSerializedCharacterList();
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

    private List<JsonCharacter> PopulateSerializedCharacterList() 
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

    private List<Character> PopulateCharacterListWithLoaded(GameData data)
    {
        List<Character> returnList = new List<Character>();
        List<JsonCharacter> serializedCharacters = data.listOfCharacters.serializedCharacters;

        if (serializedCharacters.Count > 0)
        {
            for (int index = 0; index < serializedCharacters.Count; index++)
            {
                returnList.Add(JsonCharacter.JsonToCharacter(serializedCharacters[index]));
                Debug.Log("Loaded character named: " + serializedCharacters[index].characterName);
            }

            Debug.Log("returnList.Count: " + returnList.Count);
        }

        return returnList;
    }

    public void LoadData(GameData data)
    {
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
        
        if (data.listOfCharacters.serializedCharacters.Count > 0) 
        {
            characterList.characters = PopulateCharacterListWithLoaded(data); // Shows up empty in save file because the default is null and when it is loaded on start it sets the list in GM to null which result in it saving null thus getting stuck in a cycle of loading and saving null, which in our case is nothing.
        }
        //characterArray.test = data.arrayOfCharacters.test;
        #endregion
    }

    public void SaveData(ref GameData data) 
    {
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
        serializedCharacterList.serializedCharacters = PopulateSerializedCharacterList();
        //data.arrayOfCharacters = characterArray;
        if (serializedCharacterList.serializedCharacters.Count > 0) 
        {
            data.listOfCharacters.serializedCharacters = serializedCharacterList.serializedCharacters;
        }
        //data.arrayOfCharacters.test = characterArray.test;
        #endregion
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