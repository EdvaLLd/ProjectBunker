using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIElementConsumeMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    //ett problem �r att om objektet (f�nstret) st�ngs av s� k�rs inte onpointerexit, l�sa genom att ha ett standard-
    //script f�r "close"-buttons?
    public static bool mouseOverIsAvailable = true;

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOverIsAvailable = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOverIsAvailable = true;
    }
}
