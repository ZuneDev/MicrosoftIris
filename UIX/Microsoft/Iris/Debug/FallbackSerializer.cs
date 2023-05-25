using System;
using System.Runtime.Serialization;

namespace Microsoft.Iris.Debug;

internal class FallbackSerializer : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        info.AddValue("ToString", obj?.ToString());
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        return info.GetString("ToString");
    }
}

internal class FallbackSurrogateSelector : SurrogateSelector
{
    public override ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
    {
        selector = this;
        
        if (type.Attributes.HasFlag(System.Reflection.TypeAttributes.Serializable))
            return null;

        return new FallbackSerializer();
    }
}