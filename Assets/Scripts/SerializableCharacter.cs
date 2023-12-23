using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableCharacter
{
    public bool idIsSet = false;
    #region characterTransform
    #endregion
    public SerializablePosition characterPosition;

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
    bool createNewPath = false;
    Vector3 newGoalPos;

    public string characterName;
    public List<Desease> deseases = new List<Desease>();

    public CharacterAnimation characterAnim;
    public Dictionary<Statuses, int> statuses = new Dictionary<Statuses, int>();

    public float workMultiplier = 1;

    public GameObject marker;
    public int reasonsToWarn = 0;

    public AudioClip audioClip;
    public AudioSource audioSource;
    #endregion

    [System.Serializable]
    public class SerializablePosition 
    {
        public float x;
        public float y;
        public float z;
    }
}