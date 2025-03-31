using UnityEngine;
public class LightTrigger : RegisterableAnxietyTrigger<Nyctophobia>
{
    [SerializeField] private Transform returnPoint;
    /// <summary>
    /// Bod ke kteremu hrac dojde pokud ztrati kontrolu a bude tato louce nejbliz
    /// </summary>
    public Transform ReturnPoint => returnPoint;

}
