using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockedRoom : MonoBehaviour
{
    [SerializeField]
    Pathpoint[] roomEntrances;
    [SerializeField]
    GameObject mist;
    [SerializeField]
    Item decontaminationUnit;

    private void Start()
    {
        for (int i = 0; i < roomEntrances.Length; i++)
        {
            roomEntrances[i].AddLockedFrom();
        }
    }

    private void OnMouseEnter()
    {
        if (Inventory.GetAmountOfItem(decontaminationUnit) > 0)
        {
            UIManager.clearMistBtnGO.SetActive(true);
            UIManager.clearMistBtnGO.transform.position = Camera.main.WorldToScreenPoint(transform.position);
            UIManager.clearMistBtnGO.GetComponent<Button>().onClick.RemoveAllListeners();
            UIManager.clearMistBtnGO.GetComponent<Button>().onClick.AddListener(UnlockRoom);
        }
        else
        {
            UIManager.dangerTextGO.SetActive(true);
            UIManager.dangerTextGO.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        }
    }

    private void OnMouseExit()
    {
        UIManager.clearMistBtnGO.SetActive(false);
        UIManager.dangerTextGO.SetActive(false);
    }

    public void UnlockRoom()
    {
        if (Inventory.GetAmountOfItem(decontaminationUnit) > 0)
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
            Inventory.RemoveItem(decontaminationUnit);
            UIManager.clearMistBtnGO.gameObject.SetActive(false);
            Destroy(transform.parent.gameObject);
        }
    }
}
