using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableItem : MonoBehaviour
{
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) UnitController.itemInteractedWith = this;
    }

    public abstract void InteractWith();
}
