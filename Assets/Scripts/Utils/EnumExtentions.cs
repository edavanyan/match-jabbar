using System;
using System.Reflection;
using UnityEngine;

public static class EnumExtensions
{
    public static string GetStringValue(this Enum value) {
        Type type = value.GetType();
        FieldInfo fieldInfo = type.GetField(value.ToString());

        StringValueAttribute attrib = fieldInfo.GetCustomAttribute(
            typeof(StringValueAttribute), false) as StringValueAttribute;

        return attrib.StringValue;
    }
    
    public static Color GetColor(this Enum value) {
        Type type = value.GetType();
        FieldInfo fieldInfo = type.GetField(value.ToString());

        var attrib = fieldInfo.GetCustomAttribute(
            typeof(ColorValueAttribute), false) as ColorValueAttribute;

        return attrib.ColorValue;
    }
}

public class StringValueAttribute : Attribute
{
    public string StringValue
    {
        get;
    }

    public StringValueAttribute(string val)
    {
        StringValue = val;
    }
}

public class ColorValueAttribute : Attribute
{
    public Color ColorValue
    {
        get;
    }

    public ColorValueAttribute(Color val)
    {
        ColorValue = val;
    }
}