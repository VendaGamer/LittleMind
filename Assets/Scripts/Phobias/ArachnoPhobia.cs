using System.Collections.Generic;
using UnityEngine;

public class ArachnoPhobia : MentalIllness
{
    private List<GameObject> triggersInfluencing = new();
    //
    private Dictionary<Vector3,SpiderTrigger> LastKnownPositions=new();
    //pokud hrac ztrati pavouka, ale bude se chtit vratit na misto kde ho ztratil, zacne se bat, protoze
    //"vi ze se tam nachazel"
    private void FixedUpdate()
    {
        HandleAnxiety();
    }
}