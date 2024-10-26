using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cobweb : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent<ArachnoPhobia>(out var araPho)) 
            return;
        araPho.PendNewAnxietyLevel(1f);
    }
}
