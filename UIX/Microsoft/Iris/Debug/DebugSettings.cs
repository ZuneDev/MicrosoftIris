using System.Collections.Generic;

namespace Microsoft.Iris.Debug
{
    public class DebugSettings
    {
        public bool UseDecompiler { get; set; } = false;
        public bool OpenDebugPipe { get; set; } = false;
        public List<System.Xml.XmlDocument> DecompileResults { get; } = new List<System.Xml.XmlDocument>();
        public TraceSettings TraceSettings { get; } = TraceSettings.Current;

        public Bridge Bridge { get; } =
#if ZUNE
            new(OwlCore.Remoting.RemotingMode.Host);
#else
            new();
#endif
    }
}
