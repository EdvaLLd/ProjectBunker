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

        characterList.sceneCharacters = new List<SerializableCharacter>();/*new Character[0];*/
    }

    private void Start()
    {
        characterList.sceneCharacters = PopulateCharacterList();
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

    private List<SerializableCharacter> PopulateCharacterList() 
    {
        List<SerializableCharacter> returnList = new List<SerializableCharacter>();

        // TODO - populate characterList;
        // Try using for loop.

        Character[] characters = FindObjectsOfType<Character>();

        if (characters.Length > 0) 
        {
            for (int index = 0; index < characters.Length; index++)
            {
                SerializableCharacter serializedCharacter = new SerializableCharacter();

                serializedCharacter.idIsSet = characters[index].idIsSet;
                serializedCharacter.characterPosition.x = characters[index].characterTransform.position.x;
                serializedCharacter.characterPosition.y = characters[index].characterTransform.position.y;
                serializedCharacter.characterPosition.z = characters[index].characterTransform.position.z;
                serializedCharacter.itemInteractedWithBoxCollider = characters[index].itemInteractedWithBoxCollider;
                serializedCharacter.path = characters[index].path;
                serializedCharacter.posMovingTo = characters[index].posMovingTo;
                serializedCharacter.move = characters[index].move;
                serializedCharacter.item = characters[index].item;
                serializedCharacter.task = characters[index].task;
                serializedCharacter.itemInteractedWith = characters[index].itemInteractedWith;
                //SerializedCharacter.onTaskCompletion = characters[index]._; // Solve how to get this variable.
                serializedCharacter.gearEquipped = characters[index].gearEquipped;
                serializedCharacter.hunger = characters[index].hunger;
                serializedCharacter.health = characters[index].health;
                serializedCharacter.isAlive = characters[index].isAlive;
                serializedCharacter.lowHealthWarningShowed = characters[index].lowHealthWarningShowed;
                serializedCharacter.notHungryTime = characters[index].notHungryTime;
                serializedCharacter.maxHunger = characters[index].maxHunger;
                serializedCharacter.maxHealth = characters[index].maxHealth;
                serializedCharacter.characterName = characters[index].characterName;
                serializedCharacter.deseases = characters[index].deseases;
                serializedCharacter.characterAnim = characters[index].characterAnim;
                //serializedCharacter.statuses = characters[index].statuses; // Dictionary, not supported by JsonUtility, watch tutorial for workaround.
                serializedCharacter.workMultiplier = characters[index].workMultiplier;
                serializedCharacter.marker = characters[index].marker;
                serializedCharacter.reasonsToWarn = characters[index].reasonsToWarn;
                serializedCharacter.audioClip = characters[index].audioClip;
                serializedCharacter.audioSource = characters[index].audioSource;

                // We'll fiddle with this later, after christmas.

                returnList.Add(serializedCharacter);
            }
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
        if (data.listOfCharacters.sceneCharacters.Count > 0) 
        {
            characterList.sceneCharacters = data.listOfCharacters.sceneCharacters; // Shows up empty in save file because the default is null and when it is loaded on start it sets the list in GM to null which result in it saving null thus getting stuck in a cycle of loading and saving null, which in our case is nothing.
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
        //data.arrayOfCharacters = characterArray;
        if (characterList.sceneCharacters.Count > 0) 
        {
            data.listOfCharacters.sceneCharacters = characterList.sceneCharacters;
        }
        //data.arrayOfCharacters.test = characterArray.test;
        #endregion
    }
}

[System.Serializable]
public class CharacterList
{
    public List<SerializableCharacter> sceneCharacters;
    //public Character[] sceneCharacters;
    //public int[] test;
}