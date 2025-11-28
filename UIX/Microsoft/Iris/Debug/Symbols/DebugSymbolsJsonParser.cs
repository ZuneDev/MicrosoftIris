using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Iris.Debug.Symbols;

public static class DebugSymbolsJsonParser
{
    private static readonly JsonSerializerSettings _settings = new()
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
        public override bool CanConvert(Type objectType) => objectType == typeof(SourceSpan) || objectType == typeof(SourcePosition);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is SourceSpan span)
            {
                JArray array = [$"{span.Start.Line}:{span.Start.Column}", $"{span.End.Line}:{span.End.Column}"];
                array.WriteTo(writer);
            }
            else if (value is SourcePosition position)
            {
                writer.WriteValue($"{position.Line}:{position.Column}");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(SourcePosition))
            {
                if (reader.TokenType is not JsonToken.String)
                    throw new JsonReaderException($"Expected {nameof(SourcePosition)} encoded as string");

                var positionString = reader.ReadAsString();
                ParseSourcePosition(positionString);
            }
            else if (objectType == typeof(SourceSpan))
            {
                var rangeStrs = serializer.Deserialize<IEnumerable<string>>(reader);
                var range = rangeStrs.Select(ParseSourcePosition).ToArray();
                return new SourceSpan(range[0], range[1]);
            }

            throw new NotImplementedException();

            static SourcePosition ParseSourcePosition(string positionString)
            {
                var delimiterIndex = positionString.IndexOf(':');
                if (delimiterIndex <= 0)
                    throw new JsonReaderException($"Expected {nameof(SourcePosition)} formatted as line:column");

                var line = int.Parse(positionString.Substring(0, delimiterIndex));
                var column = int.Parse(positionString.Substring(delimiterIndex + 1));
                return new SourcePosition(line, column);
            }
        }
    }
}
