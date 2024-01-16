using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;



public enum Statuses
{
    ill,
    injured,
    happy,
    sad,
    hungry,
    nothing
}

[Serializable]
public class Character : MonoBehaviour
{
    #region Variables
    #region General
    //[HideInInspector]
    public int idKey = 0;
    public bool hasName = false;
    public bool idIsSet = false;
    //public Transform characterTransform;
    public Vector3 loadedCharacterPosition;

    public BoxCollider itemInteractedWithBoxCollider = null;

    public List<Vector3> path;
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
    public float health  = 100;
    public bool isAlive = true;

    public bool lowHealthWarningShowed = false;

    public bool isHungry = true;

    //[SerializeField]
    private float hungerConsumedModifier = .3f;

    //[SerializeField]
    public float notHungryTime = 4;

    public float maxHunger;
    public float maxHealth;

    public bool createNewPath = false;
    public Vector3 newGoalPos;

    [HideInInspector]
    public List<Statuses> statuses = new List<Statuses>();
    public string characterName;

    public CharacterAnimation characterAnim { get; private set; }

    public bool isWorking;


    [SerializeField] private GameObject diseaseVFX;
    [SerializeField] private GameObject disappearSmoke;
    [SerializeField] private GameObject characterSprites;



    private void Awake()
    {
        diseaseVFX.gameObject.GetComponent<VisualEffect>().Stop();
        disappearSmoke.gameObject.GetComponent<VisualEffect>().Stop();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
        audioSource.loop = false;
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
        }
    }

    public float workMultiplier /*{ get; private set; }*/ = 1;


    [HideInInspector]
    public GameObject marker;
    [HideInInspector]
    public int reasonsToWarn = 0;

    public GearHandler gear { get; private set; } = new GearHandler();

    public MasterAura masterAura { get; private set; }

    float mood = .5f; //<.3f=ledsen, >.7f=glad
    //[SerializeField]
    public AudioClip audioClip;
    [HideInInspector]
    public AudioSource audioSource;

    //håll båda de här positiva
    float moodChangeRateIdle = 0.005f;
    float moodChangeRateWorking = 0.003f;

    #endregion
    #endregion

    private void Start()
    {
        characterAnim = GetComponentInChildren<CharacterAnimation>();
        masterAura = new MasterAura(this);
        SetLoadedPosition();
        SetCharacterName();
        SetId();
    }

    private void SetCharacterName()
    {
        if (hasName)
        {
            return;
        }

        string[] names = FindObjectOfType<GameManager>().characterNames;

        int randomNameIndex = UnityEngine.Random.Range(0, Mathf.Clamp(names.Length, 0, names.Length - 1));

        characterName = names[randomNameIndex];
        hasName = true;
    }

    public void SetLoadedPosition()
    {
        if (loadedCharacterPosition == null || loadedCharacterPosition == Vector3.zero)
        {
            Debug.LogError(gameObject.name + "'s loaded position is null. Will use default (by the entrance door).");
            gameObject.transform.position = new Vector3(5.3f, 0.362f, 0.542f);
            return;
        }
        gameObject.transform.position = loadedCharacterPosition;
    }


    /*public void UnEquipGear(GearTypes gt)
    {
        switch (gt)
        {
            case GearTypes.chest:
                if (gearEquipped.chest != null)
                {
                    Inventory.AddItem(gearEquipped.chest);
                    gearEquipped.chest = null;
                    if (characterAnim != null) { characterAnim.RemoveEquipment(GearTypes.chest); }
                }
                break;
            case GearTypes.legs:
                if (gearEquipped.legs != null)
                {
                    Inventory.AddItem(gearEquipped.boots);
                    gearEquipped.legs = null;
                    if (characterAnim != null) { characterAnim.RemoveEquipment(GearTypes.legs); }
                }
                break;
            case GearTypes.boots:
                if (gearEquipped.boots != null)
                {
                    Inventory.AddItem(gearEquipped.boots);
                    gearEquipped.chest = null;
                    if (characterAnim != null) { characterAnim.RemoveEquipment(GearTypes.boots); }
                }
                break;
            case GearTypes.weapon:
                if (gearEquipped.weapon != null)
                {
                    Inventory.AddItem(gearEquipped.weapon);
                    gearEquipped.weapon = null;
                }
                break;
            default:
                break;
        }
    }

    public void EquipGear(Equipment piece)
    {
        UnEquipGear(piece.gearType);
        switch (piece.gearType)
        {
            case GearTypes.chest:
                gearEquipped.chest = piece;
                if(characterAnim != null) {characterAnim.ChangeEquipment(GearTypes.chest, piece.gearSpriteID); }
                break;
            case GearTypes.legs:
                gearEquipped.legs = piece;
                if (characterAnim != null) { characterAnim.ChangeEquipment(GearTypes.legs, piece.gearSpriteID); }
                break;
            case GearTypes.boots:
                gearEquipped.boots = piece;
                if (characterAnim != null) { characterAnim.ChangeEquipment(GearTypes.boots, piece.gearSpriteID); }
                break;
            case GearTypes.weapon:
                gearEquipped.weapon = piece;
                break;
            default:
                break;
        }
        Inventory.RemoveItem(piece);
    }

    public GearScore GetGearScore()
    {
        GearScore totalScore = new GearScore();
        Equipment e;
        if (GearEquippedInSlot(out e, GearTypes.chest)) totalScore.armor += (e as Armor).GetDefence();
        if (GearEquippedInSlot(out e, GearTypes.legs)) totalScore.armor += (e as Armor).GetDefence();
        if (GearEquippedInSlot(out e, GearTypes.boots)) totalScore.armor += (e as Armor).GetDefence();
        if (GearEquippedInSlot(out e, GearTypes.weapon)) totalScore.attack += (e as Weapon).GetAttack();
        return totalScore;
    }

    public bool GearEquippedInSlot(out Equipment e, GearTypes gt)
    {
        switch (gt)
        {
            case GearTypes.chest:
                e = gearEquipped.chest;
                break;
            case GearTypes.legs:
                e = gearEquipped.legs;
                break;
            case GearTypes.boots:
                e = gearEquipped.boots;
                break;
            case GearTypes.weapon:
                e = gearEquipped.weapon;
                break;
            default:
                e = null;
                break;
        }
        if (e == null) return false;
        return true;
    }*/

    private void Update()
    {
        if (isAlive)
        {
            Move();
            HungerDecay();
            masterAura.Tick();
            if (isWorking)
            {
                AddMood(-moodChangeRateWorking * Time.deltaTime);
            }
            else
            {
                AddMood(moodChangeRateIdle * Time.deltaTime);
            }
        }
    }


    public void SetMood(float value)
    {
        mood = Mathf.Clamp(value, 0, 1);
        CheckMoodStatus();
    }
    public void AddMood(float value)
    {
        mood = Mathf.Clamp(mood + value, 0, 1);
        CheckMoodStatus();
    }
    //antagligen ganska ineffektivt, görs varje frame man blir glad/ledsen
    void CheckMoodStatus()
    {
        if (mood > 0.3f && mood < 0.7f)
        {
            masterAura.RemoveAuras(Debufftypes.Mood);
        }
        if (mood < 0.3f)
        {
            masterAura.AddAura(AuraPresets.Sad());
        }
        else if (mood > 0.7f)
        {
            masterAura.AddAura(AuraPresets.Happy());
        }
    }

    public bool HasStatus(Statuses status)
    {
        return statuses.Contains(status);
    }



    public void AuraValuesChanged()
    {
        /*print("-------------------");
        foreach (KeyValuePair<VariableModifiers, float> item in masterAura.aura.changeValues)
        {
            print(item.Key + " | " + item.Value);
        }*/
        float value = 0;
        masterAura.aura.GetValue(VariableModifiers.Workspeed, out value);
        workMultiplier = Mathf.Clamp(1 + value, 0.2f, 2);

        masterAura.aura.GetValue(VariableModifiers.Walkspeed, out value);
        movementSpeedMultiplier = Mathf.Clamp(1 + value, 0.2f, 2);
        CheckStatuses();
    }

    bool illVFxPlaying = false;

    void CheckStatuses()
    {
        statuses.Clear();
        characterAnim.BeNeutral();
        characterAnim.BeCured();
        VisualEffect dVFX = diseaseVFX.gameObject.GetComponent<VisualEffect>();
        if (masterAura.HasAuraWithStatus(Statuses.ill))
        {
            statuses.Add(Statuses.ill);
            characterAnim.TurnSick();
            if (!illVFxPlaying)
            {
                illVFxPlaying = true;
                dVFX.Play();
            }
        }
        else
        {
            dVFX.Stop();
            illVFxPlaying = false;
        }

        if (masterAura.HasAuraWithStatus(Statuses.injured))
        {
            statuses.Add(Statuses.injured);
        }
        if (masterAura.HasAuraWithStatus(Statuses.sad))
        {
            statuses.Add(Statuses.sad);
            characterAnim.BeSad();
        }
        if (masterAura.HasAuraWithStatus(Statuses.happy))
        {
            statuses.Add(Statuses.happy);
            characterAnim.BeHappy();
        }

        UnitController.SetCharacterStatusVisuals(this);
    }

    #region Movement and Interactions

    public void InteractedWithItem(InteractableItem item)
    {
        if (item != itemInteractedWith)
        {
            CharacterLeftTask();
            itemInteractedWith = item;
            itemInteractedWithBoxCollider = item.GetInteractableAreaCollider();
        }
        UpdateMovement(item.transform.position);
    }

    public void MoveToPos(Vector3 pos)
    {
        pos = HelperMethods.ConvertPosToBeOnGround(new Vector3(pos.x, pos.y, Pathfinding.zMoveValue), GetComponent<BoxCollider2D>().size.y * transform.lossyScale.y);
        CharacterLeftTask();

        UpdateMovement(pos);
    }

    //den här borde antagligen fixas så den blir generell och character-assignment till stationer
    //borde ske i en egen klass, men jag pallar inte (:
    public void CharacterLeftTask()
    {
        if (itemInteractedWith != null)
        {
            InteractableCraftingMachine machine;
            //borde kanske vara en generell klass och inte specifikt den här, men men
            if (itemInteractedWith.gameObject.TryGetComponent(out machine))
            {
                machine.CharacterLeftStation(this);
            }
            else
            {
                Farming farm;
                if (itemInteractedWith.gameObject.TryGetComponent(out farm))
                {
                    farm.CharacterLeftStation(this);
                }
                else
                {
                    PlayStation play;
                    if (itemInteractedWith.gameObject.TryGetComponent(out play))
                    {
                        play.CharacterLeftStation(this);
                    }
                }
            }
        }
        ResetInteractedWith();
    }

    public void ResetInteractedWith()
    {
        itemInteractedWith = null;
        itemInteractedWithBoxCollider = null;
        isWorking = false;
    }

    void UpdateMovement(Vector3 goal)
    {
        if (move)
        {
            newGoalPos = goal;
            createNewPath = true;
        }
        else
        {
            path = Pathfinding.FindPath(transform.position, goal, GetComponent<BoxCollider2D>().size.y * transform.lossyScale.y, itemInteractedWithBoxCollider);
            move = true;
            GetNextPosOnPath();

            //move behöver vara med här ifall pathen är tom
            //Animation stuff
            if (move && characterAnim != null)
            {
                characterAnim.StartMoving();
            }
        }
    }

    Vector3 GetNextPosOnPath()
    {
        if (path.Count > 0)
        {
            posMovingTo = HelperMethods.ConvertPosToBeOnGround(path[0], GetComponent<BoxCollider2D>().size.y * transform.lossyScale.y);
            path.RemoveAt(0);

            //Animation stuff
            if (characterAnim != null)
            {
                characterAnim.Flip();
            }

            return posMovingTo;
        }
        //Animation stuff
        if (characterAnim != null)
        {
            characterAnim.StopMoving();
        }
        move = false;
        return transform.position;
    }

    private void Move()
    {

        if (move) //teoretiskt s�tt f�rlorar man range p� framen man kommer fram till en point, men spelar nog ingen roll
        {
            if (Vector3.Distance(transform.position, posMovingTo) < UnitController.movementSpeed * movementSpeedMultiplier * Time.deltaTime)
            {
                transform.position = posMovingTo;
                if (createNewPath)
                {
                    path = Pathfinding.FindPath(transform.position, newGoalPos, GetComponent<BoxCollider2D>().size.y * transform.lossyScale.y, itemInteractedWithBoxCollider);
                    createNewPath = false;
                    GetNextPosOnPath();
                    return;
                }
                if (path.Count > 0)
                {
                    GetNextPosOnPath();
                }
                else
                {
                    move = false;
                    characterAnim.StopMoving();
                    if (itemInteractedWith != null)
                    {
                        onTaskCompletion?.Invoke(this);

                    }
                }
            }
            else
            {
                Vector3 newPos = Vector3.MoveTowards(transform.position, posMovingTo, UnitController.movementSpeed * movementSpeedMultiplier * Time.deltaTime);
                transform.position = newPos;
            }
        }
    }

    #endregion

    #region Hunger and Health

    void HungerDecay()
    {
        if (health != maxHealth && hunger > 80)
        {
            TakeDamage(-5 * Time.deltaTime);
            hungerConsumedModifier = 1;
        }
        else
        {
            hungerConsumedModifier = .17f;//jätteskev lösning, vet inte riktigt vad tanken var här??+
        }
        hunger -= (hungerConsumedModifier * Time.deltaTime);
        if (hunger <= 0)
        {
            TakeDamage(1 * Time.deltaTime);
        }

        health = Mathf.Clamp(health, 0, maxHealth);
        hunger = Mathf.Clamp(hunger, 0, maxHunger);
        //print(hunger);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        //TextLog.AddLog($"Unit insert name took {damage} damage and has {health} health left", MessageTypes.used);
        if (health < maxHealth / 5)
        {
            if (!lowHealthWarningShowed)
            {
                TextLog.AddLog($"Unit insert name is low on health", MessageTypes.used);
                lowHealthWarningShowed = true;
                WarnPlayer(false);
            }
        }
        else
        {
            if (lowHealthWarningShowed)
            {
                lowHealthWarningShowed = false;
                RemoveWarning();
            }
        }
        if (health == 0)
        {
            UnitDied();
        }
    }
    public void UseHunger(float amount)
    {
        hunger -= amount;
        hunger = Mathf.Clamp(hunger, 0, maxHunger);
    }
    void UnitDied()
    {
        isAlive = false;
        TextLog.AddLog("Unit died!");
        transform.position = HelperMethods.ConvertPosToBeOnGround(transform.position, GetComponent<BoxCollider2D>().size.y * transform.lossyScale.y);
        List<Character> charsInRoom = HelperMethods.GetCharactersInSameRoom(this);
        foreach (Character c in charsInRoom)
        {
            c.masterAura.AddAura(AuraPresets.Depressed());
        }
        UnitController.RemoveCharacter(this);
        reasonsToWarn = 0;
        RemoveWarning();
        CharacterLeftTask();
        if (UnitController.GetSelectedCharacter() == this)
        {
            UnitController.SwapSelectedCharacter(this);
        }

        //Prel animation stuff
        if (characterAnim != null)
        {
            //finns det någon anledning att death inte kan triggas från
            //Any State?
            characterAnim.StopReading();
            characterAnim.StopPlayingGuitar();
            characterAnim.Die();
        }
        if (UnitController.GetCharacters().Count < 1)
        {
            MainMenu.won = false;
            SceneManager.LoadScene("WinLoseScene");
        }
        StartCoroutine(Disappear());
        Destroy(gameObject, 13);
        GetComponent<BoxCollider2D>().enabled = false;
    }

    IEnumerator Disappear() {
        yield return new WaitForSeconds(10f);
        disappearSmoke.gameObject.GetComponent<VisualEffect>().Play();
        yield return new WaitForSeconds(0.5f);
        characterSprites.SetActive(false);
    }

    public void ConsumeFood(Food food)
    {
        if (maxHunger - hunger > 15)
        {
            TextLog.AddLog($"{food.DisplayName} eaten!");
            Inventory.RemoveItem(food);
            hunger = Mathf.Clamp(hunger + food.GetHungerRestoration(), 0, maxHunger);
            audioSource.Play();
        }
        if (hunger >= maxHunger)
        {
            if (isHungry)
            {
                StartCoroutine(NotHungryEffect());
            }

            TextLog.AddLog(UnitController.GetSelectedCharacter().characterName + " is not hungry.");
        }
    }

    private IEnumerator NotHungryEffect()
    {
        isHungry = false;
        float hungerConsumedModifierDefault = hungerConsumedModifier;
        hungerConsumedModifier = 0;

        yield return new WaitForSeconds(notHungryTime);

        hungerConsumedModifier = hungerConsumedModifierDefault;
        isHungry = true;
    }

    #endregion

    void WarnPlayer(bool shouldFade)
    {
        if (marker != null)
        {
            if (shouldFade)
            {
                marker.GetComponent<UIMarker>().SetShouldFade(true);
                reasonsToWarn++;
            }
            else
            {
                marker.GetComponent<UIMarker>().SetDuration(5);
            }
        }
        else
        {
            marker = UIManager.InstantiateWarningAtPos(gameObject, .6f, shouldFade, 5);
        }


        if (marker == null)
        {
            marker = UIManager.InstantiateWarningAtPos(gameObject, .6f, shouldFade, 5);
        }
        if (!shouldFade) reasonsToWarn++;
    }
    void RemoveWarning()
    {
        reasonsToWarn--;
        if (reasonsToWarn < 0) reasonsToWarn = 0;
        if (reasonsToWarn == 0 && marker != null) Destroy(marker);
    }
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && isAlive && UIElementConsumeMouseOver.mouseOverIsAvailable)
        {
            UnitController.SwapSelectedCharacter(this);
        }
    }
    public float GetCharacterDirectionX()
    {
        return transform.position.x - posMovingTo.x;
    }
    public float GetCharacterDirectionY()
    {
        return transform.position.y - posMovingTo.y;
    }
    /*public void UpdateCharacterTransform()
    {
        characterTransform = gameObject.transform;
    }*/

    private void SetId ()
    {
        if (idIsSet) 
        {
            return;
        }

        idKey = GameObject.FindObjectOfType<GameManager>().availableIdKey;
        idIsSet = true;
    }
}