using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableItem : MonoBehaviour
{
    [SerializeField]
    GameObject interactableArea;
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) UnitController.itemInteractedWith = this;
    }

    public BoxCollider GetInteractableAreaCollider()
    {
        if (interactableArea == null)
        {
            return GetComponent<BoxCollider>();
        }
        return interactableArea.GetComponent<BoxCollider>();
    }
    public abstract void InteractWith();
}
