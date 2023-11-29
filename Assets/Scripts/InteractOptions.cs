using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class InteractOptionsBools
{
    [Tooltip("If crafting is checked the item has to be parsable to Crafting Machine")]
    public bool crafting, inspect, loot, explore;
}
public class InteractOptions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //public abstract void ButtonClicked(InteractableItem item);
    [SerializeField]
    GameObject craftingGO, inspectGO, lootGO, exploreGO, paddingGO, buttonsGO, machineTextGO;

    [HideInInspector]
    public bool queueClose = false;
    bool hoveringButtons = false;

    ItemBase itemInteractedWithType;
    InteractableItem itemInteractedWith;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoveringButtons = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoveringButtons = false;
    }


    private void Update()
    {
        if (queueClose && !hoveringButtons)
        {
            CloseAllWindows();
            queueClose = false;
        }
    }

    public void SetUp(InteractOptionsBools bools, InteractableItem go, ItemBase itemInteractedWithTemp)
    {
        CloseAllWindows();
        OpenWindows(bools, go.gameObject);
        //paddingGO.transform.localScale = transform.localScale + new Vector3(0, 20, 0);
        //paddingGO.GetComponent<RectTransform>().sizeDelta = buttonsGO.GetComponent<RectTransform>().sizeDelta + new Vector2(0, 10);

        machineTextGO.GetComponent<TextMeshProUGUI>().text = itemInteractedWithTemp.DisplayName;

        queueClose = false;
        itemInteractedWithType = itemInteractedWithTemp;
        itemInteractedWith = go;
    }

    public void CloseAllWindows()
    {
        machineTextGO.SetActive(false);
        paddingGO.SetActive(false);
        for (int i = 0; i < buttonsGO.transform.childCount; i++)
        {
            buttonsGO.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void OpenWindows(InteractOptionsBools bools, GameObject go)
    {
        machineTextGO.SetActive(true);
        paddingGO.SetActive(true);
        if (bools.crafting) craftingGO.SetActive(true);
        if (bools.inspect) inspectGO.SetActive(true);
        if (bools.loot) lootGO.SetActive(true);
        if (bools.explore) exploreGO.SetActive(true);

        MoveWindowsToObject(go);
    }

    public void MoveWindowsToObject(GameObject obj)
    {
        Vector3 pos = obj.GetComponent<BoxCollider>().ClosestPointOnBounds(new Vector3(obj.transform.position.x, Mathf.Infinity));
        transform.position = Camera.main.WorldToScreenPoint(pos);
        
    }

    public void OnCraftingClick()
    {
        if (itemInteractedWith.item != null)
        {
            UnitController.InteractedWith(CharacterTasks.crafting, itemInteractedWithType, itemInteractedWith);
        }
    }

    public void OnInspectClick()
    {
        if (itemInteractedWith.item != null)
        {
            UnitController.InteractedWith(CharacterTasks.inspecting, itemInteractedWithType, itemInteractedWith);
        }
    }

    public void OnLootClick()
    {
        if (itemInteractedWith.item != null)
        {
            UnitController.InteractedWith(CharacterTasks.looting, itemInteractedWithType, itemInteractedWith);
        }
    }

    public void OnExploreClick()
    {
        if (itemInteractedWith.item != null)
        {
            UnitController.InteractedWith(CharacterTasks.exploring, itemInteractedWithType, itemInteractedWith);
        }
    }
}
