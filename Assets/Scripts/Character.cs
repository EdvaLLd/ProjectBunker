using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Character : MonoBehaviour
{
    BoxCollider itemInteractedWithBoxCollider = null;

    List<Vector3> path;
    Vector3 posMovingTo = Vector3.zero;
    bool move = false;

    public ItemBase item = null;
    public CharacterTasks task = CharacterTasks.none;
    public InteractableItem itemInteractedWith = null;

    public delegate void OnTaskCompletion(Character characterWhoFinishedTask);
    public static event OnTaskCompletion onTaskCompletion;


    //karakt�rens stats
    public float hunger = 100;
    public float health = 100;
    bool isAlive = true;
    
    private bool isHungry = true;

    [SerializeField]
    private float hungerConsumedModifier = .3f;

    [SerializeField]
    private float notHungryTime = 4;

    public float maxHunger;
    public float maxHealth;
    bool createNewPath = false;
    Vector3 newGoalPos;



    private CharacterAnimation characterAnim;

    private void Start()
    {
        maxHunger = hunger;
        maxHealth = health;

        characterAnim = GetComponentInChildren<CharacterAnimation>();
    }

    private void Update()
    {
        if (isAlive)
        {
            Move();
            HungerDecay();
        }
    }

    void HungerDecay()
    {
        if(health != maxHealth && hunger > 80)
        {
            health += 5 * Time.deltaTime;
            hungerConsumedModifier += 2;
        }
        hunger -= (hungerConsumedModifier * Time.deltaTime);
        if (hunger < 10)
        {
            health -= (10 - hunger) * Time.deltaTime * 0.3f;
            if(health < 0)
            {
                isAlive = false;
                TextLog.AddLog("Unit died!");

                //Prel animation stuff
                if(characterAnim != null)
                {
                    characterAnim.Die();
                }
            }
        }

        health = Mathf.Clamp(health, 0, maxHealth);
        hunger = Mathf.Clamp(hunger, 0, maxHunger);
    }

    public void InteractedWithItem(InteractableItem item)
    {
        CharacterLeftTask();
        itemInteractedWith = item;
        itemInteractedWithBoxCollider = item.GetInteractableAreaCollider();

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
        if(itemInteractedWith != null)
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

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && isAlive)
        {
            UnitController.SwapSelectedCharacter(this);
        }
    }

    Vector3 GetNextPosOnPath()
    {
        if (path.Count > 0)
        {
            posMovingTo = path[0];
            path.RemoveAt(0);
            return posMovingTo;
        }
        characterAnim.StopMoving();
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
                        //Animation stuff
                        if (characterAnim != null)
                        {
                            if (task == CharacterTasks.crafting)
                            {
                                characterAnim.StartCrafting();
                            }
                            else
                            {
                                characterAnim.StopCrafting();
                            }
                        }
                    }
                }
            }
            else
            {
                //Animation stuff
                if(characterAnim != null){
                    characterAnim.Flip();
                }

                Vector3 newPos = Vector3.MoveTowards(transform.position, posMovingTo, UnitController.movementSpeed * Time.deltaTime);
                transform.position = newPos;
            }
        }
    }

    public void ConsumeFood(Food food)
    {
        if (maxHunger - hunger > 15)
        {
            TextLog.AddLog($"{food.DisplayName} eaten!");
            Inventory.RemoveItem(food);
            hunger = Mathf.Clamp(hunger + food.GetHungerRestoration(), 0, maxHunger);
        }
        if (hunger >= maxHunger)
        {
            if (isHungry)
            {
                StartCoroutine(NotHungryEffect());
            }

            TextLog.AddLog(FindObjectOfType<UnitController>().GetSelectedCharacter().name + "is not hungry.");
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

    public float GetCharacterDirectionX()
    {
        return transform.position.x - posMovingTo.x;
    }

    public float GetCharacterDirectionY()
    {
        return transform.position.y - posMovingTo.y;
    }
}
