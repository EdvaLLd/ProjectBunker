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

    [SerializeField]
    private AudioClip mistClip, lockClip;
    private AudioSource audioSource;

    private void Start()
    {
        for (int i = 0; i < roomEntrances.Length; i++)
        {
            roomEntrances[i].AddLockedFrom();
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        if(mistClip != null && mist != null)
        {
            audioSource.clip = mistClip;
        }else if(lockClip != null && mist == null)
        {
            audioSource.clip = lockClip;
        }
        audioSource.spatialBlend = 1.0f;
    }

    private void OnMouseEnter()
    {
        if (UIElementConsumeMouseOver.mouseOverIsAvailable && UnitController.GetSelectedCharacter() != null)
        {
            bool characterInAdjecentRoom = false;
            foreach (Pathpoint e in roomEntrances)
            {
                if(e.isLadder)
                {
                    if(HelperMethods.AmountOfWallsBetweenPoints(e.transform.position, UnitController.GetSelectedCharacter().transform.position) < 2)
                    {
                        characterInAdjecentRoom = true;
                    }
                }
                if(!HelperMethods.WallBetweenPointsOnGround(e.transform.position, UnitController.GetSelectedCharacter().transform.position))
                {
                    characterInAdjecentRoom = true;
                }
            }
            if (Inventory.GetAmountOfItem(itemToUnlockRoom) > 0 && characterInAdjecentRoom)
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
 
            Inventory.RemoveItem(itemToUnlockRoom);
            UIManager.clearMistBtnGO.gameObject.SetActive(false);
            StartCoroutine(WaitForSound(audioSource.clip));
            
        }
    }

    private IEnumerator WaitForSound(AudioClip audio)
    {
        audioSource.Play();
        yield return new WaitForSeconds(audio.length);

        //borde antagligen göras snyggare
        if (mist != null)
        {
            Destroy(mist);
        }
        Destroy(transform.parent.gameObject);
    }
}
