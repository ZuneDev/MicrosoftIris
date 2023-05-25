﻿using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Iris.Debug;

internal class BsonFormatter : IFormatter
{
    private readonly JsonSerializer _serializer;

    public ISurrogateSelector SurrogateSelector { get; set; }

    public SerializationBinder Binder { get; set; }

    public StreamingContext Context { get; set; }

    public BsonFormatter(JsonSerializerSettings settings = null) : this(new StreamingContext(StreamingContextStates.All), settings)
    {
    }

    public BsonFormatter(StreamingContext context, JsonSerializerSettings settings = null)
    {
        Context = context;

        settings ??= new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = new JsonConverter[]
            {
                new BsonFallbackConverter()
            }
        };
        _serializer = JsonSerializer.Create(settings);
    }

    public object Deserialize(Stream serializationStream)
    {
        using JsonTextReader reader = new(new StreamReader(serializationStream))
        {
            CloseInput = false
        };

        var package = (JArray)_serializer.Deserialize(reader);

        // If the type is accessible from the current domain,
        // create an instance of it.
        var typeName = package[0].Value<string>();
        var type = Type.GetType(typeName);
        if (type != null)
            return package[1].ToObject(type, _serializer);

        return package[1];
    }

    public void Serialize(Stream serializationStream, object graph)
    {
        using JsonTextWriter writer = new(new StreamWriter(serializationStream));
        _serializer.Serialize(writer, new[] { graph?.GetType().FullName, graph });
        writer.Flush();
    }
}

internal class BsonFallbackConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return !objectType.Attributes.HasFlag(System.Reflection.TypeAttributes.Serializable)
            || objectType == typeof(uint) || objectType == typeof(ulong);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (existingValue != null)
        {
            if (objectType == typeof(uint))
                return Convert.ToUInt32(existingValue);
            else if (objectType == typeof(ulong))
                return Convert.ToUInt64(existingValue);
        }
        
        return existingValue?.ToString();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var objectType = value?.GetType();
        object serializedValue;

        if (objectType == typeof(uint))
            serializedValue = BitConverter.GetBytes((uint)value);
        else if (objectType == typeof(ulong))
            serializedValue = BitConverter.GetBytes((ulong)value);
        else
            serializedValue = value?.ToString();

        serializer.Serialize(writer, serializedValue);
    }
}
