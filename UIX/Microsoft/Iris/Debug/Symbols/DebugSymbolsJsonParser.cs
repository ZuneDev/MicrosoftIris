using Newtonsoft.Json;

namespace Microsoft.Iris.Debug.Symbols;

public static class DebugSymbolsJsonParser
{
    public static ApplicationDebugSymbols ParseForApplication(string json) => JsonConvert.DeserializeObject<ApplicationDebugSymbols>(json);

    public static FileDebugSymbols ParseForFile(string json) => JsonConvert.DeserializeObject<FileDebugSymbols>(json);

    public static string Serialize(ApplicationDebugSymbols symbols) => JsonConvert.SerializeObject(symbols);

    public static string Serialize(FileDebugSymbols symbols) => JsonConvert.SerializeObject(symbols);
}
