namespace Microsoft.Iris.Debug.Data;

public class DataMappingModel
{
    public DataMappingModel(string provider, string type, string generatedCode)
    {
        Provider = provider;
        Type = type;
        GeneratedCode = generatedCode;
    }

    public string Provider { get; internal set; }
    public string Type { get; internal set; }
    public string GeneratedCode { get; internal set; }
}
