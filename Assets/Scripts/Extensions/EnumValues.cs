using System;

public static class EnumValues
{
    public static T[] GetAllValues<T>() where T : struct, Enum
    {
        return (T[])Enum.GetValues(typeof(T));
    }
}