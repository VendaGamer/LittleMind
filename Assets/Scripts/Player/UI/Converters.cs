using UnityEngine;
using UnityEngine.UIElements;

public static class Converters
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void RegisterAllConverters()
    {
        var inverters = new ConverterGroup("Inverters");
        var converters = new ConverterGroup("Converters");
        
        inverters.AddConverter<bool, StyleEnum<DisplayStyle>>(BoolToDisplayInvert);
        converters.AddConverter<bool, StyleEnum<DisplayStyle>>(BoolToDisplay);
        ConverterGroups.RegisterConverterGroup(inverters);
        ConverterGroups.RegisterConverterGroup(converters);
    }

    private static StyleEnum<DisplayStyle> BoolToDisplayInvert(ref bool value)
        => value ? DisplayStyle.None : DisplayStyle.Flex;
    private static StyleEnum<DisplayStyle> BoolToDisplay(ref bool value)
        => value ? DisplayStyle.Flex : DisplayStyle.None;
    
}