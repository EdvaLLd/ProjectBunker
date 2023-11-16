using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableItem : MonoBehaviour
{
    [SerializeField]
    GameObject interactableArea;

    private void Start()
    {
        if (interactableArea == null)
        {
            interactableArea = new GameObject();
            interactableArea.layer = 2;
            interactableArea.transform.parent = transform;
            interactableArea.transform.localPosition = Vector3.zero;
            interactableArea.transform.rotation = transform.rotation;
            interactableArea.transform.localScale = Vector3.one;
            interactableArea.AddComponent<BoxCollider>();
            interactableArea.GetComponent<BoxCollider>().center = GetComponent<BoxCollider>().center;
            interactableArea.GetComponent<BoxCollider>().size = GetComponent<BoxCollider>().size + new Vector3(.3f, .3f, .3f);
            Debug.LogWarning($"Object \"{name}\" does not have a set interact area. Generated one based on presets");
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) UnitController.itemInteractedWith = this;
    }

    public BoxCollider GetInteractableAreaCollider()
    {
        if (interactableArea == null)
        {
            print("Shouldnt be here");
            return GetComponent<BoxCollider>();
        }
        return interactableArea.GetComponent<BoxCollider>();
    }
    public abstract void InteractWith();
}
