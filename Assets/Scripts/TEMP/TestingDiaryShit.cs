using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingDiaryShit : MonoBehaviour
    
{
    GameManager gameManager;
    [SerializeField] string  author, date;
    [SerializeField] private TextAsset diaryEntriesTextFile;
    private string[] allDiaryEntries;
    private List<string> collectedDiaryEntries;

    private int diaryIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        CreateDiaryArr();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Insert))
        {
            Debug.Log("Nu borde det fungera");
            gameManager.gameDiary.AddEntry((diaryIndex + 1) + "/15", allDiaryEntries[diaryIndex], author, date);
            diaryIndex++;
        }
    }

    public void AddDiaryEntry()
    {
        if(diaryIndex < 15)
        {
            collectedDiaryEntries.Add(allDiaryEntries[diaryIndex]);
            diaryIndex++;
        }  
    }

    private void CreateDiaryArr()
    {
        allDiaryEntries = diaryEntriesTextFile.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
    }
}
