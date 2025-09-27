using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.Iris.Debug.Symbols;

public static class DebugSymbolsJsonParser
{
    private static JsonSerializerSettings _settings = new()
    {
        Converters = [new SourceSpanJsonConverter()],
    };

    public static ApplicationDebugSymbols ParseForApplication(string json) =>
        JsonConvert.DeserializeObject<ApplicationDebugSymbols>(json, _settings);

    public static FileDebugSymbols ParseForFile(string json) =>
        JsonConvert.DeserializeObject<FileDebugSymbols>(json, _settings);

    public static string Serialize(ApplicationDebugSymbols symbols) =>
        JsonConvert.SerializeObject(symbols, _settings);

    public static string Serialize(FileDebugSymbols symbols) =>
        JsonConvert.SerializeObject(symbols, _settings);

    private class SourceSpanJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(SourceSpan);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var span = (SourceSpan)value;
            JArray array = [span.Start, span.End];
            array.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
