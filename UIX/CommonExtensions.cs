using System;
using System.Runtime.Serialization;

internal static class CommonExtensions
{
    /// <inheritdoc cref="SerializationInfo.GetValue(string, Type)"/>
    public static T GetValue<T>(this SerializationInfo info, string name)
    {
        try
        {
            return (T)info.GetValue(name, typeof(T));
        }
        catch
        {
            return default;
        }
    }
}
