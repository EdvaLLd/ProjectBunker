using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Diary
{
    public List<DiaryEntry> entries;// = new List<string>();

    [System.Serializable]
    public class DiaryEntry //: Diary
    {
        public string entryTitle;
        public string entryText;
        public string entryAuthor;
        public string entryDate;
    }
    
    public void AddEntry(string title, string text, string author, string date)
    {
        // DiaryEntry entry = new DiaryEntry();

        DiaryEntry addedEntry = new DiaryEntry();

        addedEntry.entryTitle = title;
        addedEntry.entryText = text;
        addedEntry.entryAuthor = author;
        addedEntry.entryDate = date;

        entries.Add(addedEntry);
    }

    public void UpdateDiaryGUI() 
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

        if (gameManager.gameDiary.entries.Count <= 0) 
        {
            Debug.LogWarning("No diary entries present in GameManager.");
            BlankPage(true); //true and false as in right and left, my confused friend.
            BlankPage(false);
            return;
        }

        /*int maxIndex = gameManager.gameDiary.entries.Count - 1;
        if (maxIndex <= 0) 
        {
            maxIndex = 0;
        }*/
        
        int pageIndex_Left = GameManager.leftPageIndex;
        int pageIndex_Right = GetRightPage(pageIndex_Left, gameManager.gameDiary.entries.Count);

        if (pageIndex_Left >= gameManager.gameDiary.entries.Count ) 
        {
            BlankPage(false);
        }
        /*if (pageIndex_Right >= gameManager.gameDiary.entries.Count)
        {
            BlankPage(true);
        }*/

        if (pageIndex_Left % 2 == 0)
        {
            string entryString = gameManager.gameDiary.entries[pageIndex_Left].entryText + "  - " + gameManager.gameDiary.entries[pageIndex_Left].entryAuthor + " : " + gameManager.gameDiary.entries[pageIndex_Left].entryDate;
            GameObject.Find("LeftHeaderText").GetComponent<TMPro.TextMeshProUGUI>().text = gameManager.gameDiary.entries[pageIndex_Left].entryTitle;
            GameObject.Find("LeftPageText").GetComponent<TMPro.TextMeshProUGUI>().text = entryString;
            
            //GameObject.Find("LeftPageText").GetComponent<TMPro.TextMeshProUGUI>().text = gameManager.gameDiary.entries[pageIndex_Left].entryAuthor;
            //GameObject.Find("LeftPageText").GetComponent<TMPro.TextMeshProUGUI>().text = gameManager.gameDiary.entries[pageIndex_Left].entryDate;
        }
        if (pageIndex_Right % 2 != 0) 
        {
            string entryString = gameManager.gameDiary.entries[pageIndex_Right].entryText + "  - " + gameManager.gameDiary.entries[pageIndex_Right].entryAuthor + " : " + gameManager.gameDiary.entries[pageIndex_Right].entryDate;
            GameObject.Find("RightHeaderText").GetComponent<TMPro.TextMeshProUGUI>().text = gameManager.gameDiary.entries[pageIndex_Right].entryTitle;
            GameObject.Find("RightPageText").GetComponent<TMPro.TextMeshProUGUI>().text = entryString;
            
            //GameObject.Find("RightPageText").GetComponent<TMPro.TextMeshProUGUI>().text = gameManager.gameDiary.entries[pageIndex_Right].entryAuthor;
            //GameObject.Find("RightPageText").GetComponent<TMPro.TextMeshProUGUI>().text = gameManager.gameDiary.entries[pageIndex_Right].entryDate;
        }
    }

    private int GetRightPage(int leftPage, int diaryLenght) 
    {
        int rightPage = leftPage + 1;
        if (rightPage < diaryLenght) 
        {
            return rightPage;
        }
        BlankPage(true);
        return diaryLenght-1;
    }

    private void BlankPage(bool leftOrRight)
    {
        if (leftOrRight)
        {
            GameObject.Find("RightHeaderText").GetComponent<TMPro.TextMeshProUGUI>().text = null;
            GameObject.Find("RightPageText").GetComponent<TMPro.TextMeshProUGUI>().text = null;
            
            //GameObject.Find("RightPageText").GetComponent<TMPro.TextMeshProUGUI>().text = null;
            //GameObject.Find("RightPageText").GetComponent<TMPro.TextMeshProUGUI>().text = null;
        }
        else 
        {
            GameObject.Find("LeftHeaderText").GetComponent<TMPro.TextMeshProUGUI>().text = null;
            GameObject.Find("LeftPageText").GetComponent<TMPro.TextMeshProUGUI>().text = null;
            
            //GameObject.Find("LeftPageText").GetComponent<TMPro.TextMeshProUGUI>().text = null;
            //GameObject.Find("LeftPageText").GetComponent<TMPro.TextMeshProUGUI>().text = null;
            /// No textfield present on screen for these yet.
        } 
    }
}
