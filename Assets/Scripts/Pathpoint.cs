using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Just nu är det nog bäst att ha en sån här punkt i varje dörröppning/ändring i y-led
public class Pathpoint : MonoBehaviour
{
    public bool isLocked;
    public Pathpoint[] connections;

    [HideInInspector]
    public float distFromStart = 0;
    [HideInInspector]
    public float estDistToEnd = 0;
    [HideInInspector]
    public float total = 0;
    [HideInInspector]
    public Pathpoint discoveryPoint = null;
    
    //[HideInInspector]
    //public bool mouseIsHovering = false;
    /*[HideInInspector]
    public bool allowSelection = true;*/
    //[HideInInspector]
    //public bool isSelected = false;

    //[SerializeField]
    //private Material[] selectionStateMaterials = new Material[2];

    public void ResetPoint()
    {
        distFromStart = Mathf.Infinity;
        estDistToEnd = 0;
        total = 0;
        discoveryPoint = null;
    }

    /*void OnMouseOver()
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
    }*/

    //private void SelectUnit(bool mouseAbove/*, bool canSelect*/)
    //{
        /*if (canSelect) 
        {*/
            /*if (Input.GetMouseButtonDown(1))
            {
                if (mouseAbove && !isSelected)
                {
                    isSelected = true;
                    print(gameObject.name + " was selected as end.");
                }
                else if (mouseAbove && isSelected)
                {
                    isSelected = false;
                    //print(gameObject.name + " was deselected.");
                }
                SwapMaterial();
            }
        
        //}
    }*/

    /*public void SwapMaterial() //temporary UI substitute to signify selected. 
    {
        if (isSelected)
        {
            gameObject.GetComponent<MeshRenderer>().material = selectionStateMaterials[1];
        }
        else
        {
                gameObject.GetComponent<MeshRenderer>().material = selectionStateMaterials[0];
        }
    }*/
}
