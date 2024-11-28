using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Diary : MonoBehaviour
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
    
    public bool HasMemoryNote(DiaryEntry entry)
    {
        return entries.Exists(entry => entry.memoryId == memoryId && entry.isDiscovered);
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