using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LockedRoom : MonoBehaviour
{
    [SerializeField]
    Pathpoint[] roomEntrances;
    [SerializeField]
    GameObject mist;
    [SerializeField]
    Item itemToUnlockRoom;

    private void Start()
    {
        for (int i = 0; i < roomEntrances.Length; i++)
        {
            roomEntrances[i].AddLockedFrom();
        }
    }

    private void OnMouseEnter()
    {
        if (UIElementConsumeMouseOver.mouseOverIsAvailable)
        {
            if (Inventory.GetAmountOfItem(itemToUnlockRoom) > 0)
            {
                UIManager.clearMistBtnGO.SetActive(true);
                UIManager.clearMistBtnGO.transform.position = Camera.main.WorldToScreenPoint(transform.position);
                UIManager.clearMistBtnGO.GetComponent<Button>().onClick.RemoveAllListeners();
                UIManager.clearMistBtnGO.GetComponent<Button>().onClick.AddListener(UnlockRoom);
                if (mist == null)
                {
                    UIManager.clearMistBtnGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Force entry";
                }
                else
                {
                    UIManager.clearMistBtnGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Clear mist";
                }
            }
            else
            {
                UIManager.dangerTextGO.SetActive(true);
                UIManager.dangerTextGO.transform.position = Camera.main.WorldToScreenPoint(transform.position);
                if (mist == null)
                {
                    UIManager.dangerTextGO.GetComponent<TextMeshProUGUI>().text = "LOCKED";
                }
                else
                {
                    UIManager.dangerTextGO.GetComponent<TextMeshProUGUI>().text = "DANGER!!";
                }
            }
        }
    }

    private void OnMouseExit()
    {
        UIManager.clearMistBtnGO.SetActive(false);
        UIManager.dangerTextGO.SetActive(false);
    }

    public void UnlockRoom()
    {
        if (Inventory.GetAmountOfItem(itemToUnlockRoom) > 0)
        { 
            for (int i = 0; i < roomEntrances.Length; i++)
            {
                roomEntrances[i].RemoveLockedFrom();
            }
            //borde antagligen göras snyggare
            if (mist != null)
            {
                Destroy(mist);
            }
            Inventory.RemoveItem(itemToUnlockRoom);
            UIManager.clearMistBtnGO.gameObject.SetActive(false);
            Destroy(transform.parent.gameObject);
        }
    }
}
