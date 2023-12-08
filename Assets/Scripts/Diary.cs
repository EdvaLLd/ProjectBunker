using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Diary
{
    public List<DiaryEntry> entries;// = new List<string>();

    public void AddEntry(string title, string text, string author, string date)
    {
        DiaryEntry entry = new DiaryEntry();
        
        entry.entryTitle = title;
        entry.entryText = text;
        entry.entryAuthor = author;
        entry.entryDate = date;

        entries.Add(entry);
    }
    public class DiaryEntry
    {
        public string entryTitle;
        public string entryText;
        public string entryAuthor;
        public string entryDate;
    }
}
