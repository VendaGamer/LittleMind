using System;
using System.Collections.Generic;
using UnityEngine;

public class Diary
{
    [Serializable]
    public class DiaryEntry
    {
        public string memoryId;
        public string entryText;
        public bool isDiscovered = false;
    }

    private List<DiaryEntry> entries = new();
    
    public void AddEntry(string memoryId, string entryText)
    {
        DiaryEntry newEntry = new DiaryEntry
        {
            memoryId = memoryId,
            entryText = entryText
        };
        entries.Add(newEntry);
    }
    
    public bool HasMemoryNote(string memoryId)
    {
        return entries.Exists(entry => entry.memoryId == memoryId && entry.isDiscovered);
    }
    
    public void DisplayDiary()
    {
        foreach (DiaryEntry entry in entries)
        {
            if (entry.isDiscovered)
            {
                Debug.Log($"Memory ID: {entry.memoryId}\nEntry: {entry.entryText}");
            }
        }
    }
    /// <summary>
    /// Udela zapis do deniku
    /// </summary>
    /// <param name="memoryId"></param>
    public void UnlockMemory(string memoryId)
    {
        DiaryEntry entry = entries.Find(e => e.memoryId == memoryId);
        if (entry != null)
        {
            entry.isDiscovered = true;
        }
    }
}