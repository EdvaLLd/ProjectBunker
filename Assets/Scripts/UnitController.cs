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

    private void Update()
    {
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

    private List<Pathpoint> GetPath(Vector2 start, Vector2 destination) 
    {
        return gameObject.GetComponent<Pathfinding>().FindPath(start, destination);
    }

    private void Move() 
    {
        Vector2 finalDestination = startPoint;
        direction = (/*finalDestination*/immediateDestination - startPoint).normalized;

        Pathpoint[] pointArray = FindObjectsOfType(typeof(Pathpoint)) as Pathpoint[];
        List<Pathpoint> path;//this is where it is ok, when you undo and this dissapears, you should not undo anymore.

        foreach (Pathpoint point in pointArray) 
        {
            if (point.isSelected)
            {
                finalDestination = point.transform.position;
                path = GetPath(startPoint, finalDestination);
                immediateDestination = path[index].transform.position;

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
       
         

        transform.Translate(direction * movementSpeed * Time.deltaTime);
    }
}
