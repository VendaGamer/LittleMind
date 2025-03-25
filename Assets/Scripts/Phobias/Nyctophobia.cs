using UnityEngine;

public class Nyctophobia : AnxietyManager
{

    private bool isInLight = false;
    /// <summary>
    /// Zavolano, kdyz hrac vstoupi do svetla, signalizuje uzdravovani
    /// </summary>
    public void RecoverAnxiety()
    {
        isInLight = true;
    }
}