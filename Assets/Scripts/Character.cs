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
    [Header("Character stats")]
    
    public float hunger = 100;
    public float health = 100;
    bool isAlive = true;

    [SerializeField]
    private float hungerConsumedModifier = .3f;
    [SerializeField]
    private float notHungryTime = 4;


    private bool isHungry = true;

    public float maxHunger;
    public float maxHealth;
    bool createNewPath = false;
    Vector3 newGoalPos;

    //blir dubbla det här värdet
    float maxDistToGroundCheck = 10;

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
        pos = ConvertPosToBeOnGround(new Vector3(pos.x, pos.y, Pathfinding.zMoveValue));

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
            path = FindAndAdaptPath(transform.position, goal);
            GetNextPosOnPath();
            move = true;

            //Animation stuff
            if (characterAnim != null)
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

    List<Vector3> FindAndAdaptPath(Vector3 startPos, Vector3 goalPos)
    {
        List<Vector3> tempPath = Pathfinding.FindPath(startPos, goalPos);

        for (int i = 0; i < tempPath.Count - 1; i++)
        {
            tempPath[i] = ConvertPosToBeOnGround(tempPath[i]);
        }
        if (WallBetweenPoints(startPos, tempPath[0]))
        {
            tempPath.InsertRange(0, FixPathBetweenPoints(startPos, tempPath[0]));
        }
        if (tempPath.Count > 1)
        {
            if (itemInteractedWithBoxCollider != null)
            {
                tempPath[tempPath.Count - 1] = itemInteractedWithBoxCollider.ClosestPoint(tempPath[tempPath.Count - 2]);
            }
            if (WallBetweenPoints(tempPath[tempPath.Count - 1], tempPath[tempPath.Count - 2]))
            {
                tempPath.InsertRange(tempPath.Count - 1, FixPathBetweenPoints(tempPath[tempPath.Count - 2], tempPath[tempPath.Count - 1]));
                if (itemInteractedWithBoxCollider != null)
                {
                    tempPath[tempPath.Count - 1] = itemInteractedWithBoxCollider.ClosestPoint(tempPath[tempPath.Count - 2]);
                }
            }
        }
        return tempPath;
    }

    List<Vector3> FixPathBetweenPoints(Vector3 p1, Vector3 p2)
    {
        List<Vector3> result = new List<Vector3>();
        Vector3 t = p1;
        t.z = Pathfinding.zMoveValue;
        t = ConvertPosToBeOnGround(t);
        result.Add(t);

        t = p2;
        t.z = Pathfinding.zMoveValue;
        t = ConvertPosToBeOnGround(t);
        result.Add(t);
        return result;
    }

    bool WallBetweenPoints(Vector3 p1, Vector3 p2)
    {
        Vector3 dir = (p2 - p1).normalized;
        float length = Vector3.Distance(p1, p2);
        return Physics.Raycast(p1, dir, length, 1 << 6);
    }

    Vector3 ConvertPosToBeOnGround(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.BoxCast(pos, new Vector3(.5f, .01f, .5f), Vector3.down, out hit, Quaternion.identity, maxDistToGroundCheck, 1 << 6))
        {
            float groundPosY = hit.point.y;
            float characterHeight = transform.lossyScale.y;
            pos = new Vector3(pos.x, groundPosY + (characterHeight / 2), pos.z);
        }
        return pos;
    }

    Vector3 GetNextPosOnPath()
    {
        if (path.Count > 0)
        {
            posMovingTo = path[0];
            path.RemoveAt(0);
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
                if (createNewPath)
                {
                    path = FindAndAdaptPath(transform.position, newGoalPos);
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
        if(hunger != maxHunger)
        {
            TextLog.AddLog($"{food.DisplayName} eaten!");
            Inventory.RemoveItem(food);
            hunger = Mathf.Clamp(hunger + food.GetHungerRestoration(), 0, maxHunger);
        }
        if(hunger >= maxHunger)
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

    public float GetCharacterDirectionX()
    {
        return transform.position.x - posMovingTo.x;

    }
}
