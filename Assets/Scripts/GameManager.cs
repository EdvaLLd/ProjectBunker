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

    [SerializeField]
    private bool spawnStartCharacter = false;
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
    //[HideInInspector]
    public int availableIdKey = 0;
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

        if (isFirstRun && spawnStartCharacter)
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

    private void SpawnSerializedCharacters(List<JsonCharacter> serializedCharacters)
    {
        if (serializedCharacters.Count <= 0)
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


        GameObject spawnCharacter = characterPrefab; // initialized GameObject with prefab.

        Character[] sceneCharacters = GameObject.FindObjectsOfType<Character>(); // Creating Character array of all characters present on scene.
        int[] sceneIds = new int[sceneCharacters.Length];
        bool alreadyExists = false/*new bool[serializedCharacters.Count]*/;// bool to confirm or deny existance of characters on scene.

        for (int keyIndex = 0; keyIndex < sceneCharacters.Length; keyIndex++)
        {
            sceneIds[keyIndex] = sceneCharacters[keyIndex].idKey;

            if (keyIndex >= Mathf.Clamp(sceneCharacters.Length - 1, 0, sceneCharacters.Length - 1))
            {
                break;
            }
        }

        for (int characterIndex = 0; characterIndex < serializedCharacters.Count; characterIndex++)  // looping over serialized characters.
        {
            for (int compareIndex = 0; compareIndex < sceneIds.Length; compareIndex++) // looping over scene Characters to compare to serialized characters.
            {
                Character sceneCharacter = sceneCharacters[compareIndex];

                Debug.Log("serializedCharID: " + serializedCharacters[characterIndex].idKey + " vs " + "sceneCharID: " + sceneIds[compareIndex]); // sceneCharacter.idKey is set to 1 for some reason?

                if (serializedCharacters[characterIndex].idKey != sceneIds[compareIndex]) // comparing if idKey do not match.
                {
                    Debug.Log("No match.");
                }
                else // if not.
                {
                    Debug.Log("Match.");
                    OverwriteCharacter(serializedCharacters[characterIndex], sceneCharacter);
                    /*alreadyExists[characterIndex]*/
                    alreadyExists = true;
                    break;
                }
                if (compareIndex >= Mathf.Clamp(sceneCharacters.Length - 1, 0, sceneCharacters.Length))
                {
                    break;
                }
            }

            if (!alreadyExists)
            {
                Debug.Log(serializedCharacters[characterIndex].idKey + " was summoned!");
                JsonCharacter.JsonToCharacter(serializedCharacters[characterIndex], spawnCharacter.GetComponent<Character>());
                Instantiate(spawnCharacter);
            }
            else

            if (characterIndex >= Mathf.Clamp(serializedCharacters.Count - 1, 0, serializedCharacters.Count - 1))
            {
                break;
            }
        }

        /*for (int spawnIndex = 0; spawnIndex < serializedCharacters.Count; spawnIndex++) 
        {
          
        }*/

        /*string s = "alreadyExists: ";
        foreach (bool b in alreadyExists) 
        {
            s += b + " ";
        }
        Debug.Log(s);*/
    }

    private void SpawnStartCharacter()
    {
        if (characterPrefab == null)
        {
            Debug.LogError("No prefab set for characterPrefab. \nSpawning of character was canceled.");
            return;
        }

        GameObject character = Instantiate(characterPrefab);

        character.transform.position = new Vector3(5.3f, 0.362f, 0.542f);
    }

    private void OverwriteCharacter(JsonCharacter inputCharacterData, Character targetCharacter)
    {
        targetCharacter.idKey = inputCharacterData.idKey;
        targetCharacter.hasName = inputCharacterData.hasName;
        targetCharacter.idIsSet = inputCharacterData.idIsSet;
        targetCharacter.loadedCharacterPosition = inputCharacterData.storedCharacterPosition;
        targetCharacter.itemInteractedWithBoxCollider = inputCharacterData.itemInteractedWithBoxCollider;
        targetCharacter.path = inputCharacterData.path;
        targetCharacter.posMovingTo = inputCharacterData.posMovingTo;
        targetCharacter.move = inputCharacterData.move;
        targetCharacter.item = inputCharacterData.item;
        targetCharacter.task = inputCharacterData.task;
        targetCharacter.itemInteractedWith = inputCharacterData.itemInteractedWith;
        //SerializedCharacter.onTaskCompletion = characters[index]._; // Solve how to get this variable.
        //serializedCharacter.gearEquipped = sceneCharacter.gearEquipped;
        targetCharacter.hunger = inputCharacterData.hunger;
        targetCharacter.health = inputCharacterData.health;
        targetCharacter.isAlive = inputCharacterData.isAlive;
        targetCharacter.lowHealthWarningShowed = inputCharacterData.lowHealthWarningShowed;
        targetCharacter.notHungryTime = inputCharacterData.notHungryTime;
        targetCharacter.maxHunger = inputCharacterData.maxHunger;
        targetCharacter.maxHealth = inputCharacterData.maxHealth;
        targetCharacter.createNewPath = inputCharacterData.createNewPath;
        targetCharacter.newGoalPos = inputCharacterData.newGoalPos;
        targetCharacter.characterName = inputCharacterData.characterName;
        //serializedCharacter.deseases = sceneCharacter.deseases;
        //targetCharacter.characterAnim = inputCharacter.characterAnim;
        targetCharacter.statuses = inputCharacterData.statuses.content;
        targetCharacter.workMultiplier = inputCharacterData.workMultiplier;
        targetCharacter.marker = inputCharacterData.marker;
        targetCharacter.reasonsToWarn = inputCharacterData.reasonsToWarn;
        targetCharacter.audioClip = inputCharacterData.audioClip;
        targetCharacter.audioSource = inputCharacterData.audioSource;

        targetCharacter.SetLoadedPosition();

        Debug.Log("Character with idKey: " + targetCharacter.idKey + " was overwritten");
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
        int[] keys = new int[serializedCharacterList.serializedCharacters.Count];

        for (int index = 0; index < keys.Length; index++)
        {
            keys[index] = (serializedCharacterList.serializedCharacters[index].idKey);

            if (index >= keys.Length)
            {
                break;
            }
        }

        availableIdKey = Mathf.Max(keys) + 1;
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