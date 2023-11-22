using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIElementConsumeMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    //ett problem är att om objektet (fönstret) stängs av så körs inte onpointerexit, lösa genom att ha ett standard-
    //script för "close"-buttons?
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
