public interface ISetting<out T> where T : struct
{
    public T CurrentValue { get; }
    public T AppliedValue { get; }
    
    public bool CanApplyOrReset { get; }

    public void Apply();

    public void Reset();
}