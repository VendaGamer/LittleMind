using System;
using System.Collections.Generic;
using UnityEngine;
public class SpiderTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent<ArachnoPhobia>(out var araPho))
            return;
        araPho.IncreaseAnxiety(0.05f);
    }
}
