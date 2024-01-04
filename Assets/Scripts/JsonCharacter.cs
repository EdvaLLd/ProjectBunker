using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JsonCharacter
{
    public bool idIsSet = false;
    public bool hasName = false;

    //public JsonSerializablePosition characterPosition;
    public Vector3 storedCharacterPosition;
    //public Transform transform;

    public BoxCollider itemInteractedWithBoxCollider = null;

    public List<Vector3> path;
    public Vector3 posMovingTo = Vector3.zero;
    public bool move = false;

    public ItemBase item = null;
    public CharacterTasks task = CharacterTasks.none;
    public InteractableItem itemInteractedWith = null;

    public delegate void OnTaskCompletion(Character characterWhoFinishedTask);
    public static event OnTaskCompletion onTaskCompletion;

    public EqippedGearSet gearEquipped;

    #region CharacterStats
    public float hunger = 100;
    public float health = 100;
    public bool isAlive = true;

    public bool lowHealthWarningShowed = false;

    public bool isHungry = true;
    public float notHungryTime = 4;

    public float maxHunger;
    public float maxHealth;
    public bool createNewPath = false;
    public Vector3 newGoalPos;

    //public JsonSerializablePosition newGoalPos;

    public string characterName;
    public List<Desease> deseases = new List<Desease>();

    public CharacterAnimation characterAnim;
    public JsonUtilityAddon.JsonDictionary.SerializableDictionary<Statuses, int> statuses = new JsonUtilityAddon.JsonDictionary.SerializableDictionary<Statuses, int>();
        //<Statuses, int> statuses = new Dictionary<Statuses, int>();

    public float workMultiplier = 1;

    public GameObject marker;
    public int reasonsToWarn = 0;

    public AudioClip audioClip;
    public AudioSource audioSource;
    #endregion

    [System.Serializable]
    public class JsonSerializablePosition 
    {
        public float x;
        public float y;
        public float z;
    }

    #region Character/JsonCharacter conversion
    public static JsonCharacter CharacterToJson(Character sceneCharacter)
    {
        
        Debug.Log("Saved character named: " + sceneCharacter.characterName);

        JsonCharacter serializedCharacter = new JsonCharacter();

        //character.UpdateCharacterTransform();
        //serializedCharacter.transform = character.characterTransform;
        serializedCharacter.hasName = sceneCharacter.hasName;
        serializedCharacter.idIsSet = sceneCharacter.idIsSet;
        serializedCharacter.storedCharacterPosition = sceneCharacter.transform.position;
        serializedCharacter.itemInteractedWithBoxCollider = sceneCharacter.itemInteractedWithBoxCollider;
        serializedCharacter.path = sceneCharacter.path;
        serializedCharacter.posMovingTo = sceneCharacter.posMovingTo;
        serializedCharacter.move = sceneCharacter.move;
        serializedCharacter.item = sceneCharacter.item;
        serializedCharacter.task = sceneCharacter.task;
        serializedCharacter.itemInteractedWith = sceneCharacter.itemInteractedWith;
        //SerializedCharacter.onTaskCompletion = characters[index]._; // Solve how to get this variable.
        serializedCharacter.gearEquipped = sceneCharacter.gearEquipped;
        serializedCharacter.hunger = sceneCharacter.hunger;
        serializedCharacter.health = sceneCharacter.health;
        serializedCharacter.isAlive = sceneCharacter.isAlive;
        serializedCharacter.lowHealthWarningShowed = sceneCharacter.lowHealthWarningShowed;
        serializedCharacter.notHungryTime = sceneCharacter.notHungryTime;
        serializedCharacter.maxHunger = sceneCharacter.maxHunger;
        serializedCharacter.maxHealth = sceneCharacter.maxHealth;
        serializedCharacter.createNewPath = sceneCharacter.createNewPath;
        serializedCharacter.newGoalPos = sceneCharacter.newGoalPos;
        serializedCharacter.characterName = sceneCharacter.characterName;
        serializedCharacter.deseases = sceneCharacter.deseases;
        serializedCharacter.characterAnim = sceneCharacter.characterAnim;
        serializedCharacter.statuses = JsonUtilityAddon.JsonDictionary.DictionaryToJson(sceneCharacter.statuses);
        serializedCharacter.workMultiplier = sceneCharacter.workMultiplier;
        serializedCharacter.marker = sceneCharacter.marker;
        serializedCharacter.reasonsToWarn = sceneCharacter.reasonsToWarn;
        serializedCharacter.audioClip = sceneCharacter.audioClip;
        serializedCharacter.audioSource = sceneCharacter.audioSource;

        return serializedCharacter;
    }

    public static Character JsonToCharacter(JsonCharacter serializedCharacter)
    {
        Character returnCharacter = new Character();

        //character.characterTransform.position = Vector3.zero;

        returnCharacter.hasName = serializedCharacter.hasName;
        returnCharacter.idIsSet = serializedCharacter.idIsSet;
        //returnCharacter.characterTransform.position = serializedCharacter.characterPosition;
        returnCharacter.loadedCharacterPosition = serializedCharacter.storedCharacterPosition;
        returnCharacter.itemInteractedWithBoxCollider = serializedCharacter.itemInteractedWithBoxCollider;
        returnCharacter.path = serializedCharacter.path;
        returnCharacter.posMovingTo = serializedCharacter.posMovingTo;
        returnCharacter.move = serializedCharacter.move;
        returnCharacter.item = serializedCharacter.item;
        returnCharacter.task = serializedCharacter.task;
        returnCharacter.itemInteractedWith = serializedCharacter.itemInteractedWith;
        //SerializedCharacter.onTaskCompletion = characters[index]._; // Solve how to get this variable.
        returnCharacter.gearEquipped = serializedCharacter.gearEquipped;
        returnCharacter.hunger = serializedCharacter.hunger;
        returnCharacter.health = serializedCharacter.health;
        returnCharacter.isAlive = serializedCharacter.isAlive;
        returnCharacter.lowHealthWarningShowed = serializedCharacter.lowHealthWarningShowed;
        returnCharacter.notHungryTime = serializedCharacter.notHungryTime;
        returnCharacter.maxHunger = serializedCharacter.maxHunger;
        returnCharacter.maxHealth = serializedCharacter.maxHealth;
        returnCharacter.createNewPath = serializedCharacter.createNewPath;
        returnCharacter.newGoalPos = serializedCharacter.newGoalPos;
        returnCharacter.characterName = serializedCharacter.characterName;
        returnCharacter.deseases = serializedCharacter.deseases;
        returnCharacter.characterAnim = serializedCharacter.characterAnim;        

        returnCharacter.statuses = serializedCharacter.JsonToStatuses(serializedCharacter.statuses);
        returnCharacter.workMultiplier = serializedCharacter.workMultiplier;
        returnCharacter.marker = serializedCharacter.marker;
        returnCharacter.reasonsToWarn = serializedCharacter.reasonsToWarn;
        returnCharacter.audioClip = serializedCharacter.audioClip;
        returnCharacter.audioSource = serializedCharacter.audioSource;

        return returnCharacter;
    }

    /*public static*/private Dictionary<Statuses, int> JsonToStatuses(JsonUtilityAddon.JsonDictionary.SerializableDictionary<Statuses,int> serializedDictionary) 
    {
        Dictionary<Statuses, int> returnDictionary = new Dictionary<Statuses, int>();

        //Debug.LogError("Am I alive???");

        for (int index = 0; index < serializedDictionary.keyArray.Length; index++) 
        {
            returnDictionary.Add(serializedDictionary.keyArray[index], serializedDictionary.valueArray[index]);

            if (index >= serializedDictionary.keyArray.Length)
            {
                break;
            }
        }

        return returnDictionary;
    }
    #endregion
}