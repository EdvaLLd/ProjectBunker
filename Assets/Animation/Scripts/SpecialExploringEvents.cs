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
    [SerializeField] private string eventName;

    [SerializeField] private Transform birthPos;

    void Start()
    {
        eventPanel.SetActive(false);
        characterChoicePanel.SetActive(false);
        Debug.Log(birthPos);
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            ShowSpecialEvent("hej");
            ShowCharacterChoice();
        }
    }

    public void ShowSpecialEvent(string eventText)
    {
        Time.timeScale = 0; /*Det här fungerar inte optimalt så jag vet inte om vi borde ha det här eller inte*/
        string editedText = eventText.Replace("name", eventName).Replace("Name", eventName);
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
        newCharacter = Instantiate(newCharacter, birthPos.position, Quaternion.identity);
        newCharacter.GetComponentInChildren<PartsChanger>().RandomizeCharacter();
    }
}
