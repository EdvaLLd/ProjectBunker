using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class InteractOptionsBools
{
    [Tooltip("If crafting is checked the item has to be parsable to Crafting Machine")]
    public bool crafting, inspect, loot, explore, eat;
}
public class InteractOptions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //public abstract void ButtonClicked(InteractableItem item);
    [SerializeField]
    GameObject craftingGO, inspectGO, lootGO, exploreGO, eatGO, buttonsGO, machineTextGO;

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
        machineTextGO.GetComponent<TextMeshProUGUI>().text = itemInteractedWithTemp.DisplayName;
        OpenWindows(bools, go.gameObject);


        queueClose = false;
        itemInteractedWithType = itemInteractedWithTemp;
        itemInteractedWith = go;
    }

    public void CloseAllWindows()
    {
        machineTextGO.SetActive(false);
        for (int i = 0; i < buttonsGO.transform.childCount; i++)
        {
            buttonsGO.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void OpenWindows(InteractOptionsBools bools, GameObject go)
    {
        MoveWindowsToObject(go);
        machineTextGO.SetActive(true);
        if (bools.crafting) craftingGO.SetActive(true);
        if (bools.inspect) inspectGO.SetActive(true);
        if (bools.loot) lootGO.SetActive(true);
        if (bools.explore) exploreGO.SetActive(true);
        if (bools.eat) eatGO.SetActive(true);

    }

    public void MoveWindowsToObject(GameObject obj)
    {
        Vector3 dir = obj.transform.position - Camera.main.transform.position;
        //kryssprodukt simplifierad om man alltid har vektorerna dir och (1,0,0)
        dir = new Vector3(0, dir.z, dir.y);
        if (dir.y <= 0) dir = -dir;
        Vector3 pos = obj.GetComponent<BoxCollider>().ClosestPointOnBounds(obj.transform.position + dir*1000) - dir.normalized/10;
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

    public void OnEatClick()
    {
        if (itemInteractedWith.item != null)
        {
            UnitController.InteractedWith(CharacterTasks.eating, itemInteractedWithType, itemInteractedWith);
        }
    }
}
