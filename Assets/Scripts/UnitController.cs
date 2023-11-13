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

    List<Vector2> path = new List<Vector2>();
    Vector2 posMovingTo = Vector2.zero;
    bool move = false;

    private void Update()
    {
        if (isSelected && itemInteractedWith != null)
        {

            if (Vector2.Distance(transform.position, itemInteractedWith.transform.position) < 0.1f)
            {
                itemInteractedWith.InteractWith();
            }
            path = GetPath(transform.position, itemInteractedWith.transform.position);
            if (!move && path.Count > 0)
            {
                posMovingTo = path[0];
                path.RemoveAt(0);
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
    private Vector2 GetUnitPosition()
    {
        //if (!isSelected) {return new Vector3(0,0,0);}
        return gameObject.transform.position;
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

    private List<Vector2> GetPath(Vector2 start, Vector2 destination) 
    {
        return gameObject.GetComponent<Pathfinding>().FindPath(start, destination);
    }


    private void Move() 
    {
        /*Vector2 finalDestination = startPoint;
        direction = (immediateDestination - startPoint).normalized;

        Pathpoint[] pointArray = FindObjectsOfType(typeof(Pathpoint)) as Pathpoint[];
        List<Vector2> path;//this is where it is ok, when you undo and this dissapears, you should not undo anymore.

        foreach (Pathpoint point in pointArray) 
        {
            if (point.isSelected)
            {
                finalDestination = point.transform.position;
                path = GetPath(startPoint, finalDestination);
                immediateDestination = path[index];

                if (gameObject.GetComponent<Collider2D>().OverlapPoint(immediateDestination) && index < path.Count)
                {
                    startPoint = GetUnitPosition();
                    //point.isSelected = false;
                    //point.GetComponent<Pathpoint>().SwapMaterial();
                    index++;
                }
                if (gameObject.GetComponent<Collider2D>().OverlapPoint(finalDestination))
                {
                    startPoint = finalDestination;
                    point.isSelected = false;
                    point.GetComponent<Pathpoint>().SwapMaterial();
                    index = 0;
                }
                print("index: " + index+ ", maxIndex: " + path.Count);
            }
        }
       
         

        transform.Translate(direction * movementSpeed * Time.deltaTime);*/

        if(move) //teoretiskt sätt förlorar man range på framen man kommer fram till en point, men spelar nog ingen roll
        {
            if(Vector2.Distance(transform.position, posMovingTo) < movementSpeed * Time.deltaTime)
            {
                transform.position = new Vector3(posMovingTo.x, posMovingTo.y, transform.position.z);
                if (path.Count > 0)
                {
                    posMovingTo = path[0];
                    path.RemoveAt(0);
                }
                else move = false;
            }
            else
            {
                Vector2 newPos = Vector2.MoveTowards(transform.position, posMovingTo, movementSpeed * Time.deltaTime);
                transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
            }
        }
    }

}
