using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [HideInInspector]
    public bool mouseIsHovering = false;
    [HideInInspector]
    public bool isSelected = false;

    [SerializeField]
    private Material[] selectionStateMaterials = new Material[2];

    [SerializeField]
    private float movementSpeed = 1;

    private Vector2 startPoint;
    private Vector2 immediateDestination;
    private Vector2 direction;

    private int index = 0;

    public static InteractableItem itemInteractedWith = null;
    BoxCollider itemInteractedWithBoxCollider = null;

    List<Vector3> path;
    Vector3 posMovingTo = Vector3.zero;
    bool move = false;

    private void Update()
    {
        if (isSelected && itemInteractedWith != null)
        {

            if (Vector3.Distance(transform.position, itemInteractedWith.transform.position) < 0.1f)
            {
                itemInteractedWith.InteractWith();
            }

            itemInteractedWithBoxCollider = itemInteractedWith.GetInteractableAreaCollider();

            path = GetPath(transform.position, itemInteractedWith.transform.position);

            if (!move)
            {
                GetNextPosOnPath();
            }
            itemInteractedWith = null;
            move = true;
        }
        Move();
    }

    void OnMouseOver()
    {
        if (!mouseIsHovering)
        {
           //print(gameObject.name + " can be selected.");
            mouseIsHovering = true;
        }

        SelectUnit(mouseIsHovering);
    }

    void OnMouseExit()
    {
        if (mouseIsHovering)
        {
            //print(gameObject.name + " can no longer be selected.");
            mouseIsHovering = false;
        }
    }

    Vector3 GetNextPosOnPath()
    {
        if(path.Count > 0)
        {
            if(path.Count == 1)
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

    private void SelectUnit(bool canSelect)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (canSelect && !isSelected)
            {
                isSelected = true;
                //startPoint = GetUnitPosition();
                //print(gameObject.name + " was selected.");
            }
            else if (canSelect && isSelected)
            {
                isSelected = false;
                //print(gameObject.name + " was deselected.");
            }
            SwapMaterial();
        }
    }

    private void SwapMaterial() //temporary UI substitute to signify selected. 
    {
        if (isSelected) 
        {
            gameObject.GetComponent<MeshRenderer>().material = selectionStateMaterials[1];
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material = selectionStateMaterials[0];
        }
    }

    private List<Vector3> GetPath(Vector3 start, Vector3 destination) 
    {
        return gameObject.GetComponent<Pathfinding>().FindPath(start, destination);
    }


    private void Move() 
    {

        if(move) //teoretiskt sätt förlorar man range på framen man kommer fram till en point, men spelar nog ingen roll
        {
            if(Vector3.Distance(transform.position, posMovingTo) < movementSpeed * Time.deltaTime)
            {
                transform.position = posMovingTo;//new Vector3(posMovingTo.x, posMovingTo.y, transform.position.z);
                if (path.Count > 0)
                {
                    GetNextPosOnPath();
                }
                else move = false;
            }
            else
            {
                Vector3 newPos = Vector3.MoveTowards(transform.position, posMovingTo, movementSpeed * Time.deltaTime);
                transform.position = newPos;//new Vector3(newPos.x, newPos.y, transform.position.z);
            }
        }
    }

}
