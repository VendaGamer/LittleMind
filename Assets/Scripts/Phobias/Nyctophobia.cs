using System.Linq;
using UnityEngine;

public class Nyctophobia : AnxietyManager
{

    protected override void Update()
    {
        if (activeAnxietySources.Count == 0)
        {
            IncreaseAnxiety(0.01f);
        }
    }

    public override void UnRegisterAnxietySource(IAnxietySource anxietySource)
    {
        if (FadeCoroutine != null)
        {
            StopCoroutine(FadeCoroutine);
        }
        activeAnxietySources.Remove(anxietySource);
        OnAnxietySourcesChanged();
    }

    public override void RegisterAnxietySource(IAnxietySource anxietySource)
    {
        if (activeAnxietySources.Count == 0)
        {
            StartFadeAnxiety();
        }
        activeAnxietySources.Add(anxietySource);
        OnAnxietySourcesChanged();
    }
}