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
        float hungerConsumedModifier = .3f;
        if(health != maxHealth && hunger > 80)
        {
            health += 5 * Time.deltaTime;
            hungerConsumedModifier += 2;
        }
        hunger -= (hungerConsumedModifier * Time.deltaTime)/3;
        if (hunger < 20)
        {
            health -= (20 - hunger) * Time.deltaTime;
            if(health < 0)
            {
                isAlive = false;
                TextLog.AddLog("Unit died!");
            }
        }

        health = Mathf.Clamp(health, 0, maxHealth);
        hunger = Mathf.Clamp(hunger, 0, maxHunger);
    }

    public void InteractedWithItem(InteractableItem item)
    {
        itemInteractedWith = item;
        itemInteractedWithBoxCollider = item.GetInteractableAreaCollider();

        UpdateMovement(item.transform.position);
    }

    public void MoveToPos(Vector3 pos)
    {
        itemInteractedWith = null;
        itemInteractedWithBoxCollider = null;
        pos = HelperMethods.ConvertPosToBeOnGround(new Vector3(pos.x, pos.y, Pathfinding.zMoveValue), transform.lossyScale.y);

        UpdateMovement(pos);
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
            path = Pathfinding.FindPath(transform.position, goal, transform.lossyScale.y, itemInteractedWithBoxCollider);
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
                    path = Pathfinding.FindPath(transform.position, newGoalPos, transform.lossyScale.y, itemInteractedWithBoxCollider);
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
        TextLog.AddLog($"{food.DisplayName} eaten!");
        if(maxHunger != hunger)
        {
            Inventory.RemoveItem(food);
            hunger = Mathf.Clamp(hunger + food.GetHungerRestoration(), 0, maxHunger);
        }
        else
        {
            print("me no hungry");
        }
    }

    public float GetCharacterDirectionX()
    {
        return transform.position.x - posMovingTo.x;
    }
}
