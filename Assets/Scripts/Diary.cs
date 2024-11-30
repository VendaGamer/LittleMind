using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Diary : MonoBehaviour
{
    [SerializeField]
    private List<DiaryEntry> entries = new();
    
    public void AddEntry([CanBeNull] string entryLabel, string entryText)
    {
        DiaryEntry newEntry = new DiaryEntry
        {
            
            entryText = entryText
        };
        entries.Add(newEntry);
    }
    
    [Serializable]
    public class DiaryEntry
    {
        [CanBeNull] public string entryLabel;
        public string entryText;
        public bool isDiscovered = false;
    }
}