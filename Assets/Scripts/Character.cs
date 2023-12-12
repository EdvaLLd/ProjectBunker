using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System;

[Serializable]
public class EqippedGearSet
{
    public Equipment chest = null;
    public Equipment legs = null;
    public Equipment boots = null;
    public Equipment weapon = null;
}

public enum Statuses
{
    ill,
    injured,
    happy,
    sad,
    hungry,
}
public class Character : MonoBehaviour
{

    #region Variables

    BoxCollider itemInteractedWithBoxCollider = null;

    List<Vector3> path;
    Vector3 posMovingTo = Vector3.zero;
    bool move = false;

    public ItemBase item = null;
    public CharacterTasks task = CharacterTasks.none;
    public InteractableItem itemInteractedWith = null;

    public delegate void OnTaskCompletion(Character characterWhoFinishedTask);
    public static event OnTaskCompletion onTaskCompletion;

    EqippedGearSet gearEquipped;

    //karakt�rens stats
    public float hunger = 100;
    public float health = 100;
    bool isAlive = true;

    bool lowHealthWarningShowed = false;
    
    private bool isHungry = true;

    [SerializeField]
    private float hungerConsumedModifier = .3f;

    [SerializeField]
    private float notHungryTime = 4;

    public float maxHunger;
    public float maxHealth;
    bool createNewPath = false;
    Vector3 newGoalPos;

    List<Desease> deseases = new List<Desease>();

    Dictionary<Statuses, int> statuses = new Dictionary<Statuses, int>();


    private CharacterAnimation characterAnim;

    public float workMultiplier { get; private set; } = 1;


    GameObject marker;
    int reasonsToWarn = 0;

    [SerializeField]
    private AudioClip audioClip;
    private AudioSource audioSource;

    #endregion

    private void Start()
    {
        //maxHunger = hunger;
        //maxHealth = health;

        gearEquipped = new EqippedGearSet();
        characterAnim = GetComponentInChildren<CharacterAnimation>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
        audioSource.loop = false;
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
        }
    }


    public void UnEquipGear(GearTypes gt)
    {
        switch (gt)
        {
            case GearTypes.chest:
                if (gearEquipped.chest != null)
                {
                    Inventory.AddItem(gearEquipped.chest);
                    gearEquipped.chest = null;
                }
                break;
            case GearTypes.legs:
                if (gearEquipped.legs != null)
                {
                    Inventory.AddItem(gearEquipped.boots);
                    gearEquipped.legs = null;
                }
                break;
            case GearTypes.boots:
                if (gearEquipped.boots != null)
                {
                    Inventory.AddItem(gearEquipped.boots);
                    gearEquipped.chest = null;
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
                break;
            case GearTypes.legs:
                gearEquipped.legs = piece;
                break;
            case GearTypes.boots:
                gearEquipped.boots = piece;
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
    }

    private void Update()
    {
        if (isAlive)
        {
            Move();
            HungerDecay();
            DeseaseTick();
        }
    }




    #region Statuses
    public void AddStatus(Statuses status)
    {
        if (statuses.ContainsKey(status)) statuses[status] += 1;
        else 
        { 
            statuses.Add(status, 1);
            TextLog.AddLog("\"insert character name\" is now " + status);
            UnitController.SetCharacterStatusVisuals(this);
            WarnPlayer(true);
        }
        CheckWorkMultiplier();
    }
    public bool HasStatus(Statuses status)
    {
        if (statuses.ContainsKey(status)) return true;
        return false;
    }
    public void RemoveStatus(Statuses status)
    {
        if (statuses.ContainsKey(status)) 
        { 
            statuses[status] -= 1;
            if (statuses[status] < 1)
            {
                statuses.Remove(status);
                UnitController.SetCharacterStatusVisuals(this);
                TextLog.AddLog("\"insert character name\" is no longer " + status);
            }
        }
        else Debug.LogError("shouldnt be here");

        CheckWorkMultiplier();
    }

    #endregion

    #region Deseases
    void DeseaseTick()
    {
        for (int i = deseases.Count - 1; i >= 0; i--)
        {
            deseases[i].Tick();
        }
    }
    public void AddDesease<T>() where T : Desease, new()
    {
        if(!HasDesease<T>(out _))
        {
            T d = new T();
            d.SetCharacter(this);
            deseases.Add(d);
            TextLog.AddLog("\"insert character name\" contracted " + d.GetType() + "!");
            AddStatus(Statuses.ill);
        }
    }
    public void RemoveDesease(Desease desease)
    {
        if (desease.GetHealth() < 0)
        {
            TextLog.AddLog("\"insert character name\" survived her " + desease + " infection!");
            deseases.Remove(desease);
            RemoveStatus(Statuses.ill);
        }
    }

    public bool HasDesease<T>(out T desease) where T : Desease
    {
        desease = null;
        foreach (Desease d in deseases)
        {
            if(d.GetType() == typeof(T))
            {
                desease = d as T;
                return true;
            }
        }
        return false;
    }

    public List<Desease> GetDeseases()
    {
        return deseases;
    }

    //för om vi medicerar olika deseases
    public void TreatDesease(Desease desease)
    {
        Item med = Database.GetItemWithID("01013");
        //farligt sätt att göra grejer
        if (Inventory.GetAmountOfItem(med) > 0)
        {
            desease.Medicate();
            Inventory.RemoveItem(med);
        }
    }

    //för om vi medicerar alla deseases
    public void TreatDesease()
    {
        Item med = Database.GetItemWithID("01013");
        //farligt sätt att göra grejer
        if (Inventory.GetAmountOfItem(med) > 0 && deseases.Count > 0)
        {
            for (int i = deseases.Count - 1; i >= 0; i--)
            {
                deseases[i].Medicate();
            }
            Inventory.RemoveItem(med);
        }
    }

    #endregion

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
        CharacterLeftTask();
        pos = HelperMethods.ConvertPosToBeOnGround(new Vector3(pos.x, pos.y, Pathfinding.zMoveValue), transform.lossyScale.y);

        UpdateMovement(pos);
    }

    void CharacterLeftTask()
    {
        if (itemInteractedWith != null)
        {
            InteractableCraftingMachine machine;
            //borde kanske vara en generell klass och inte specifikt den här, men men
            if (itemInteractedWith.gameObject.TryGetComponent(out machine))
            {
                machine.CharacterLeftStation(this);
            }
        }
        itemInteractedWith = null;
        itemInteractedWithBoxCollider = null;
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
            posMovingTo = path[0];
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
            if (Vector3.Distance(transform.position, posMovingTo) < UnitController.movementSpeed * Time.deltaTime)
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
                Vector3 newPos = Vector3.MoveTowards(transform.position, posMovingTo, UnitController.movementSpeed * Time.deltaTime);
                transform.position = newPos;
            }
        }
    }

    #endregion

    #region Hunger and Health

    void HungerDecay()
    {
        if(health != maxHealth && hunger > 80)
        {
            TakeDamage(-5 * Time.deltaTime);
            hungerConsumedModifier = 2;
        }
        else
        {
            hungerConsumedModifier = 0.3f;//jätteskev lösning, vet inte riktigt vad tanken var här??+
        }
        hunger -= (hungerConsumedModifier * Time.deltaTime);
        if (hunger < 10)
        {
            TakeDamage((10 - hunger) * Time.deltaTime * 0.3f);
        }

        health = Mathf.Clamp(health, 0, maxHealth);
        hunger = Mathf.Clamp(hunger, 0, maxHunger);
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
        UnitController.RemoveCharacter(this);
        reasonsToWarn = 0;
        RemoveWarning();

        if (UnitController.GetSelectedCharacter() == this)
        {
            UnitController.SwapSelectedCharacter(this);
        }

        //Prel animation stuff
        if (characterAnim != null)
        {
            characterAnim.Die();
        }
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

            TextLog.AddLog(UnitController.GetSelectedCharacter().name + "is not hungry.");
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

    void CheckWorkMultiplier()
    {
        //sortera dessa på hur negativa de är (värre är överst)
        if (HasStatus(Statuses.ill) || HasStatus(Statuses.injured) || HasStatus(Statuses.sad))
        {
            workMultiplier = 0.1f;
        }
        else
        {
            workMultiplier = 1;
        }
    }
    void WarnPlayer(bool shouldFade)
    {
        if(marker != null)
        {
            if(shouldFade)
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
        if(!shouldFade)  reasonsToWarn++;
    }
    void RemoveWarning()
    {
        reasonsToWarn--;
        if(reasonsToWarn < 0) reasonsToWarn = 0;
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

}
