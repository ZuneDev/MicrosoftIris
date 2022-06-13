using System.Collections.Generic;

#if NET40_OR_GREATER || NET6_0_OR_GREATER
using DataMappingModelsList = System.Collections.ObjectModel.ObservableCollection<Microsoft.Iris.Debug.DataMappingModel>;
#else
using DataMappingModelsList = System.Collections.Generic.List<Microsoft.Iris.Debug.DataMappingModel>;
#endif

namespace Microsoft.Iris.Debug
{
    public class DebugSettings
    {
        public bool UseDecompiler { get; set; } = false;
        public bool OpenDebugPipe { get; set; } = false;
        public List<System.Xml.XmlDocument> DecompileResults { get; } = new List<System.Xml.XmlDocument>();
        public TraceSettings TraceSettings { get; } = TraceSettings.Current;

        public bool GenerateDataMappingModels { get; set; } = true;
        public DataMappingModelsList DataMappingModels { get; } = new DataMappingModelsList();

        public Bridge Bridge { get; } =
#if OPENZUNE
            new(OwlCore.Remoting.RemotingMode.Host);
#else
            new();
#endif
    }

    public class DataMappingModel
    {
        internal DataMappingModel(string provider, string type, string generatedCode)
        {
            Provider = provider;
            Type = type;
            GeneratedCode = generatedCode;
        }

        public string Provider { get; internal set; }
        public string Type { get; internal set; }
        public string GeneratedCode { get; internal set; }
    }
}
