using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ItemHoverDesc : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
    bool isHovering = false;
    public ItemBase item;


    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        if (item != null)
        {
            UIManager.SetWindowActive(UIManager.hoverWindow);
            UIManager.hoverWindow.transform.position = Input.mousePosition;
            UIManager.hoverWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.DisplayName;
            UIManager.hoverWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.Description;
        }
        else
        {
            print("uh oh");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        UIManager.CloseWindow(UIManager.hoverWindow);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if(isHovering)
        {
            UIManager.hoverWindow.transform.position = Input.mousePosition;
        }
    }
}
