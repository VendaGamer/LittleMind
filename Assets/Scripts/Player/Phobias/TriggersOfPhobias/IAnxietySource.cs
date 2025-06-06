public interface IAnxietySource
{
    public float AnxietyAmount { get; }
    /// <summary>
    /// Represent anxiety value that will be use to increase anxiety on Registration
    /// </summary>
    public float InitialAnxietyAmount { get; }
}