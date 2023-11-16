using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    BoxCollider itemInteractedWithBoxCollider = null;

    List<Vector3> path;
    Vector3 posMovingTo = Vector3.zero;
    bool move = false;

    private void Update()
    {
        Move();
    }

    public void InteractedWithItem(InteractableItem item)
    {
        if (Vector3.Distance(transform.position, item.GetInteractableAreaCollider().ClosestPoint(transform.position)) < 0.1f)
        {
            item.InteractWith();
        }

        itemInteractedWithBoxCollider = item.GetInteractableAreaCollider();

        path = Pathfinding.FindPath(transform.position, item.transform.position);

        if (!move)
        {
            GetNextPosOnPath();
        }

        move = true;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
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
                posMovingTo.y = itemInteractedWithBoxCollider.transform.position.y; //det här borde antagligen göras om
                //till att gå på marken
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

        if (move) //teoretiskt sätt förlorar man range på framen man kommer fram till en point, men spelar nog ingen roll
        {
            if (Vector3.Distance(transform.position, posMovingTo) < UnitController.movementSpeed * Time.deltaTime)
            {
                transform.position = posMovingTo;
                if (path.Count > 0)
                {
                    GetNextPosOnPath();
                }
                else move = false;
            }
            else
            {
                Vector3 newPos = Vector3.MoveTowards(transform.position, posMovingTo, UnitController.movementSpeed * Time.deltaTime);
                transform.position = newPos;
            }
        }
    }
}
