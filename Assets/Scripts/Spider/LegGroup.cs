using System.Collections.Generic;

[System.Serializable]
public class LegGroup
{
    public string name;
    public int order;
    public List<IKFootSolver> legs = new List<IKFootSolver>();
}