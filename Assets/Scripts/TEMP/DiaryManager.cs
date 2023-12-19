using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiaryManager : MonoBehaviour
    
{
    [SerializeField] private TextMeshProUGUI lHeaderText, lEntryText, rHeaderText, rEntryText;
    [SerializeField] private TextAsset diaryEntriesTextFile;
    private string[] allDiaryEntries;
    private int collectedDiaryEntries = 0;

    private int diaryIndex = 0;

    void Start()
    {
        CreateDiaryArr();
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Insert))
        //{
        //    AddDiaryEntry();
        //}
    }

    public void AddDiaryEntry()
    {
        if(collectedDiaryEntries < 15)
        {
            collectedDiaryEntries++;
            TextLog.AddLog("A new diary entry!");
            if(collectedDiaryEntries == 15)
            {
                CharacterExplorationUI.foundParadise = true;
            }
        }  
    }

    public void NextPage()
    {
        if(diaryIndex + 2 < collectedDiaryEntries && diaryIndex <= 13)
        {
            diaryIndex += 2;
        }
    }

    public void PreviousPage()
    {
        if(diaryIndex - 2 >= 0)
        {
            diaryIndex -= 2;
        }
    }

    public void UpdateDiary()
    {
        if(collectedDiaryEntries == 0)
        {
            lEntryText.text = "You haven't collected any diary entries yet.";
            lHeaderText.text = null;
            rHeaderText.text = null;
            rEntryText.text = null;
            return;
        }
        if(collectedDiaryEntries %2 == 0 || diaryIndex + 2 < collectedDiaryEntries)
        {
            lHeaderText.text = (diaryIndex + 1) + "/15";
            lEntryText.text = allDiaryEntries[diaryIndex];
            rHeaderText.text = (diaryIndex + 2) + "/15";
            rEntryText.text = allDiaryEntries[diaryIndex+1];
        }
        else
        {
            lHeaderText.text = (diaryIndex + 1) + "/15";
            lEntryText.text = allDiaryEntries[diaryIndex];
            rHeaderText.text = null;
            rEntryText.text = null;
        }
        
    }
    
    private void CreateDiaryArr()
    {
        allDiaryEntries = diaryEntriesTextFile.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
    }

}
