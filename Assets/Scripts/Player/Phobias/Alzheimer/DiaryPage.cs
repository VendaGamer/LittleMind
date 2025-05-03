using UnityEngine;

public class DiaryPage : MonoBehaviour
{
    [field: SerializeField] public Pages page { get; private set; }
    void Start()
    {
        enabled = false;
    }
    //TODO: Nazvy stranek

    public enum Pages
    { 
        first,
        second,
        third,
        fourth,
        fifth,
    }
}
