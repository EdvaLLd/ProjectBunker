using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JsonCharacter
{
    #region General
    public int idKey;
    public bool idIsSet = false;
    public bool hasName = false;

    //public JsonSerializablePosition characterPosition;
    public Vector3 storedCharacterPosition;
    //public Transform transform;

    public BoxCollider itemInteractedWithBoxCollider = null;

    public /*JsonUtilityAddon.JsonList.Serializable*/List<Vector3> path = new();
    public Vector3 posMovingTo = Vector3.zero;
    public bool move = false;

    public ItemBase item = null;
    public CharacterTasks task = CharacterTasks.none;
    public InteractableItem itemInteractedWith = null;

    public delegate void OnTaskCompletion(Character characterWhoFinishedTask);
    public static event OnTaskCompletion onTaskCompletion;

    float movementSpeedMultiplier = 1;
    #endregion
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

    public CharacterAnimation characterAnim;
    //public JsonUtilityAddon.JsonDictionary.SerializableDictionary<Statuses, int> statuses = new JsonUtilityAddon.JsonDictionary.SerializableDictionary<Statuses, int>();
    //<Statuses, int> statuses = new Dictionary<Statuses, int>();

    public JsonUtilityAddon.JsonList.SerializableList<Statuses> statuses = new();

    public float workMultiplier = 1;

    public GameObject marker;
    public int reasonsToWarn = 0;

    public AudioClip audioClip;
    public AudioSource audioSource;
    #endregion

    /*[System.Serializable]
    public class JsonSerializablePath
    {
        public List<JsonUtilityAddon.JsonVector.SerializableVector3> pathPoints;
    }*/

    #region Character/JsonCharacter conversion
    public static JsonCharacter CharacterToJson(Character sceneCharacter)
    {
        
        Debug.Log("Saved character named: " + sceneCharacter.characterName);

        JsonCharacter serializedCharacter = new JsonCharacter();

        //character.UpdateCharacterTransform();
        //serializedCharacter.transform = character.characterTransform;
        serializedCharacter.idKey = sceneCharacter.idKey;
        serializedCharacter.hasName = sceneCharacter.hasName;
        serializedCharacter.idIsSet = sceneCharacter.idIsSet;
        serializedCharacter.storedCharacterPosition = sceneCharacter.gameObject.transform.position;
        serializedCharacter.itemInteractedWithBoxCollider = sceneCharacter.itemInteractedWithBoxCollider;
        serializedCharacter.path/*.content*/ = sceneCharacter.path;
        serializedCharacter.posMovingTo = sceneCharacter.posMovingTo;
        serializedCharacter.move = sceneCharacter.move;
        serializedCharacter.item = sceneCharacter.item;
        serializedCharacter.task = sceneCharacter.task;
        serializedCharacter.itemInteractedWith = sceneCharacter.itemInteractedWith;
        //SerializedCharacter.onTaskCompletion = characters[index]._; // Solve how to get this variable.
        //serializedCharacter.gearEquipped = sceneCharacter.gearEquipped;
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
        //serializedCharacter.deseases = sceneCharacter.deseases;
        serializedCharacter.characterAnim = sceneCharacter.characterAnim;
        serializedCharacter.statuses.content = sceneCharacter.statuses;
        serializedCharacter.workMultiplier = sceneCharacter.workMultiplier;
        serializedCharacter.marker = sceneCharacter.marker;
        serializedCharacter.reasonsToWarn = sceneCharacter.reasonsToWarn;
        serializedCharacter.audioClip = sceneCharacter.audioClip;
        serializedCharacter.audioSource = sceneCharacter.audioSource;

        return serializedCharacter;
    }

    public static void JsonToCharacter(JsonCharacter serializedCharacter, Character targetCharacter) // Will apply the following variable values from the serialized JsonCharacter to Character targetCharacter.
    {
        //Character returnCharacter = new Character(); /// Not allowed because can not create scripts deriving from MonoBehaviour using "new" keyword as they act as components, not objects if I indestand this correctly.

        //character.characterTransform.position = Vector3.zero;
        targetCharacter.idKey = serializedCharacter.idKey;
        targetCharacter.hasName = serializedCharacter.hasName;
        targetCharacter.idIsSet = serializedCharacter.idIsSet;
        //returnCharacter.characterTransform.position = serializedCharacter.characterPosition;
        targetCharacter.loadedCharacterPosition = serializedCharacter.storedCharacterPosition;
        targetCharacter.itemInteractedWithBoxCollider = serializedCharacter.itemInteractedWithBoxCollider;
        targetCharacter.path = serializedCharacter.path/*.content*/;
        targetCharacter.posMovingTo = serializedCharacter.posMovingTo;
        targetCharacter.move = serializedCharacter.move;
        targetCharacter.item = serializedCharacter.item;
        targetCharacter.task = serializedCharacter.task;
        targetCharacter.itemInteractedWith = serializedCharacter.itemInteractedWith;
        //SerializedCharacter.onTaskCompletion = characters[index]._; // Solve how to get this variable.
        //returnCharacter.gearEquipped = serializedCharacter.gearEquipped;
        targetCharacter.hunger = serializedCharacter.hunger;
        targetCharacter.health = serializedCharacter.health;
        targetCharacter.isAlive = serializedCharacter.isAlive;
        targetCharacter.lowHealthWarningShowed = serializedCharacter.lowHealthWarningShowed;
        targetCharacter.notHungryTime = serializedCharacter.notHungryTime;
        targetCharacter.maxHunger = serializedCharacter.maxHunger;
        targetCharacter.maxHealth = serializedCharacter.maxHealth;
        targetCharacter.createNewPath = serializedCharacter.createNewPath;
        targetCharacter.newGoalPos = serializedCharacter.newGoalPos;
        targetCharacter.characterName = serializedCharacter.characterName;
        //returnCharacter.deseases = serializedCharacter.deseases;
        //returnCharacter.characterAnim = serializedCharacter.characterAnim;        

        targetCharacter.statuses = serializedCharacter.statuses.content;
        targetCharacter.workMultiplier = serializedCharacter.workMultiplier;
        targetCharacter.marker = serializedCharacter.marker;
        targetCharacter.reasonsToWarn = serializedCharacter.reasonsToWarn;
        targetCharacter.audioClip = serializedCharacter.audioClip;
        targetCharacter.audioSource = serializedCharacter.audioSource;

        //return returnCharacter;
    }
    #endregion
}