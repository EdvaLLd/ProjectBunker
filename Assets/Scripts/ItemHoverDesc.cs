using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public enum DescDirEnum
{
    TopRight,
    Top,
    TopLeft,
    Left,
    BottomLeft,
    Bottom,
    BottomRight,
    Right
}

public class ItemHoverDesc : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
    bool isHovering = false;
    public ItemBase item;
    Vector3 paddingToCursor = new Vector3(5, 5, 0);

    [SerializeField]
    DescDirEnum descDir = DescDirEnum.TopRight;



    Vector2 descWindowSize;
    Vector2 descDirToV2;
    Vector3 windowPosDif;
    private void Start()
    {
        RectTransform rt = UIManager.hoverWindow.GetComponent<RectTransform>();
        descWindowSize = rt.anchorMax - rt.anchorMin + rt.sizeDelta; //tror den här raden är rätt?

        switch (descDir)
        {
            case DescDirEnum.TopRight:
                descDirToV2 = new Vector2(1, 1);
                break;
            case DescDirEnum.Top:
                descDirToV2 = new Vector2(0, 1);
                break;
            case DescDirEnum.TopLeft:
                descDirToV2 = new Vector2(-1, 1);
                break;
            case DescDirEnum.Left:
                descDirToV2 = new Vector2(-1, 0);
                break;
            case DescDirEnum.BottomLeft:
                descDirToV2 = new Vector2(-1, -1);
                break;
            case DescDirEnum.Bottom:
                descDirToV2 = new Vector2(0, -1);
                break;
            case DescDirEnum.BottomRight:
                descDirToV2 = new Vector2(1, -1);
                break;
            case DescDirEnum.Right:
                descDirToV2 = new Vector2(1, 0);
                break;
            default:
                descDirToV2 = new Vector2(1, 1);
                break;
        }
        CanvasScaleChange();
        UIManager.onCanvasScaleFactorChange += CanvasScaleChange;
    }

    void CanvasScaleChange()
    {
        windowPosDif = new Vector2((descWindowSize.x/2 + paddingToCursor.x) * descDirToV2.x, (descWindowSize.y / 2 + paddingToCursor.y) * descDirToV2.y) * UIManager.canvasScaleFactor;
    }
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
            UIManager.hoverWindow.transform.position = Input.mousePosition + windowPosDif;
            UIManager.MoveObjectToScreen(UIManager.hoverWindow);
        }
    }
}
