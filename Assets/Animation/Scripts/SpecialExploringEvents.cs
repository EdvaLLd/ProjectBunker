using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using TMPro;

public class SpecialExploringEvents : MonoBehaviour
{
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private TextMeshProUGUI eventPanelText;
    [SerializeField] private string eventName;

    void Start()
    {
        eventPanel.SetActive(false);
    }

    public void ShowSpecialEvent(string eventText)
    {
        Time.timeScale = 0; /*Det h�r fungerar inte optimalt s� jag vet inte om vi borde ha det h�r eller inte*/
        string editedText = eventText.Replace("name", eventName).Replace("Name", eventName);
        eventPanelText.text = editedText;
        eventPanel.SetActive(true);
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
    }
}
