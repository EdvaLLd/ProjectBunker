using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using TMPro;

public class SpecialExploringEvents : MonoBehaviour
{
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private GameObject characterChoicePanel;
    [SerializeField] private TextMeshProUGUI eventPanelText;
    [SerializeField] private GameObject newCharacter;

    [SerializeField] private Transform birthPos;

    void Start()
    {
        eventPanel.SetActive(false);
        characterChoicePanel.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            ShowSpecialEvent("Hejdå", "Sture");
            ShowCharacterChoice();
        }
    }

    public void ShowSpecialEvent(string eventText, string characterName)
    {
        Time.timeScale = 0; /*Det här fungerar inte optimalt så jag vet inte om vi borde ha det här eller inte*/
        string editedText = eventText.Replace("name", characterName).Replace("Name", characterName);
        eventPanelText.text = editedText;
        eventPanel.SetActive(true);
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
    }

    public void ShowCharacterChoice()
    {
        characterChoicePanel.SetActive(true);
    }

    public void CreateNewCharacter()
    {
        GameObject temp = Instantiate(newCharacter, birthPos.position, Quaternion.identity);
        temp.transform.GetChild(0).gameObject.SetActive(true);

        temp.GetComponentInChildren<PartsChanger>().RandomizeCharacter();
        UnitController.AddCharacter(temp.GetComponent<Character>());
    }
}
