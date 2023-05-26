using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
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
        using BsonDataReader reader = new(serializationStream)
        {
            CloseInput = false
        };

        BsonPackage package = new(_serializer, reader);
        return package.Object ?? package.SerializedObject;
    }

    public void Serialize(Stream serializationStream, object graph)
    {
        using BsonDataWriter writer = new(serializationStream);
        _serializer.Serialize(writer, new[] { graph?.GetType().FullName, graph });
        writer.Flush();
    }

    private class BsonPackage
    {
        public BsonPackage(JsonSerializer serializer, JsonReader reader)
        {
            var package = (JObject)serializer.Deserialize(reader);

            TypeName = package["0"].Value<string>();
            SerializedObject = (JObject)package["1"];

            // If the type is accessible from the current domain,
            // create an instance of it.
            var type = Type.GetType(TypeName);
            if (type != null)
                Object = SerializedObject.ToObject(type, serializer);
        }

        public string TypeName { get; set; }

        public JObject SerializedObject { get; set; }

        public object Object { get; set; }
    }
}

internal class BsonFallbackConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return !objectType.Attributes.HasFlag(System.Reflection.TypeAttributes.Serializable)
            || objectType.IsEnum || objectType == typeof(uint) || objectType == typeof(ulong)
            || objectType == typeof(Type);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object jsonValue, JsonSerializer serializer)
    {
        jsonValue = reader.Value;
        if (objectType == reader.ValueType)
            return jsonValue;

        if (objectType == typeof(uint))
            return BitConverter.ToUInt32((byte[])jsonValue, 0);
        else if (objectType == typeof(ulong))
            return BitConverter.ToUInt64((byte[])jsonValue, 0);
        else if (objectType.IsEnum)
        {
            if (reader.ValueType == typeof(string))
                return Enum.Parse(objectType, (string)reader.Value);

            return Enum.ToObject(objectType, reader.Value);
        }
        else if (objectType == typeof(Type))
        {
            if (jsonValue is not string typeName)
                return null;

            return Type.GetType(typeName);
        }
        
        return jsonValue?.ToString();
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
