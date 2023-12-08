using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Diary
{
    public List<string> diaryEntries = ;

    public void AddEntry(string text, List<string> diary) 
    {
        diary.Add(text);
    }
}
