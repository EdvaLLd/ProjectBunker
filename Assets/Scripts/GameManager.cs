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

    public ExplorationEvents.StandardExploreEvent[] standardExploreEvents;
    public ExplorationEvents.ExploreEvent[] mainExploreEvents;
    public ExplorationEvents.RandomExploreEvent[] randomExploreEvents;
    public ExplorationEvents.LimitedExploreEvent[] limitedExploreEvents;
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
    public static int unusedCharacterIndex = 0;
    public static CharacterArray characterArray = new CharacterArray();
    #endregion
    #region Time
    private float hour = 6;
    private float minute;
    #endregion
    #endregion


    private void Awake()
    {
        instance = this;
        
        skyboxManager = GameObject.FindObjectOfType<SkyboxController>();

        explorableLocations = Locations.SetExplorableLocations();

        //this.gameDiary.UpdateDiaryGUI();

        characterArray.sceneCharacters = new Character[0];
    }

    private void Update()
    {
        skyboxManager.DayAndNightCycle(skyboxManager.cycleRate);
        DigitalClock();
    }

    private void DigitalClock() 
    {
        SkyboxController skyboxManager = GameObject.FindObjectOfType<SkyboxController>();

        //SetMinute(skyboxManager);
        //SetHour(skyboxManager);
        
        minute += skyboxManager.cycleRate * 24/360 * 60 * Time.deltaTime;
        if (minute >= 60) { minute = 0; hour++; }
        
        //hour += skyboxManager.cycleRate / 60;
        if (hour >= 24) { hour = 0; }

        string displayTime = Mathf.FloorToInt(hour).ToString("00") + ":" + Mathf.FloorToInt(minute).ToString("00");

        GameObject.Find("DayTimeStamp").transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = displayTime;
    }

    public static Locations.Location[] GetExplorableLocations() 
    {
        return explorableLocations;
    }

    public void LoadData(GameData data)
    {
        #region Time
        hour = data.clockHour;
        minute = data.clockMinute;
        #endregion

        #region Diary
        leftPageIndex = data.diaryLeftPageIndex;
        #endregion

        #region Event
        eventIndex = data.mainEventIndex;
        #endregion

        #region Character
        unusedCharacterIndex = data.freeCharacterIndex;
        characterArray = data.arrayOfCharacters;
        //characterArray.sceneCharacters = data.arrayOfCharacters.sceneCharacters;
        #endregion
    }

    public void SaveData(ref GameData data) 
    {
        #region Time
        data.clockHour = hour;
        data.clockMinute = minute;
        #endregion

        #region Diary
        data.diaryLeftPageIndex = leftPageIndex;
        #endregion

        #region Event
        data.mainEventIndex = eventIndex;
        #endregion

        #region Character
        data.freeCharacterIndex = unusedCharacterIndex;
        data.arrayOfCharacters = characterArray;
        //data.arrayOfCharacters.sceneCharacters = characterArray.sceneCharacters;
        #endregion
    }
}

[System.Serializable]
public class CharacterArray
{
    public Character[] sceneCharacters;
}