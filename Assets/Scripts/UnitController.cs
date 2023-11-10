using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [HideInInspector]
    public bool selectionAllowed = false;
    [HideInInspector]
    public bool isSelected = false;

    [SerializeField]
    private Material[] testMaterials = new Material[2];

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    void OnMouseOver()
    {
        if (!selectionAllowed)
        {
           //print(gameObject.name + " can be selected.");
            selectionAllowed = true;
        }

        SelectUnit(selectionAllowed);
    }

    void OnMouseExit()
    {
        if (selectionAllowed)
        {
            //print(gameObject.name + " can no longer be selected.");
            selectionAllowed = false;
        }
    }

    private void SelectUnit(bool canSelect)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (canSelect && !isSelected)
            {
                isSelected = true;
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
            gameObject.GetComponent<MeshRenderer>().material = testMaterials[1];
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material = testMaterials[0];
        }
    }
}
