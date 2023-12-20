using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class InteractableItem : MonoBehaviour
{
    [SerializeField]
    GameObject interactableArea;

    //[SerializeField]
    public ItemBase item;

    static GameObject interactOptions;

    [SerializeField]
    InteractOptionsBools interactOptionsBools;


    private void Awake()
    {
        if (interactOptions == null)
        {
            interactOptions = GameObject.FindGameObjectWithTag("InteractOptions");
        }
    }

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


    private void OnMouseEnter()
    {
        if (UIElementConsumeMouseOver.mouseOverIsAvailable && UnitController.GetSelectedCharacter() != null)
        {
            interactOptions.GetComponent<InteractOptions>().SetUp(interactOptionsBools, this, item);
        }
    }

    private void OnMouseExit()
    {
        interactOptions.GetComponent<InteractOptions>().queueClose = true;
    }

    public BoxCollider GetInteractableAreaCollider()
    {
        if (interactableArea == null)
        {
            print("Shouldnt be here (on object "+name+")");
            return GetComponent<BoxCollider>();
        }
        return interactableArea.GetComponent<BoxCollider>();
    }
    //public abstract void InteractWith();
}
