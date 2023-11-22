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


    float maxHunger;
    float maxHealth;

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

        //print($"{hunger} | {health}");
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

        path = Pathfinding.FindPath(transform.position, item.transform.position);

        if (!move)
        {
            GetNextPosOnPath();
        }

        move = true;

        //Animation stuff
        if(characterAnim != null){
            characterAnim.StartMoving();
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
            if (path.Count == 1)
            {
                posMovingTo = itemInteractedWithBoxCollider.ClosestPoint(transform.position);
                posMovingTo.y = itemInteractedWithBoxCollider.transform.position.y; //det h�r borde antagligen g�ras om
                //till att g� p� marken
                path.RemoveAt(0);
            }
            else
            {
                posMovingTo = path[0];
                path.RemoveAt(0);
            }
            return posMovingTo;
        }
        print("Should never be here");
        return transform.position;
    }

    private void Move()
    {

        if (move) //teoretiskt s�tt f�rlorar man range p� framen man kommer fram till en point, men spelar nog ingen roll
        {
            

            if (Vector3.Distance(transform.position, posMovingTo) < UnitController.movementSpeed * Time.deltaTime)
            {
                transform.position = posMovingTo;
                if (path.Count > 0)
                {
                    GetNextPosOnPath();
                }
                else
                {
                    move = false;
                    onTaskCompletion?.Invoke(this);

                    //Animation stuff
                    if(characterAnim != null){
                        characterAnim.StopMoving();
                        if(task == CharacterTasks.crafting)
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
