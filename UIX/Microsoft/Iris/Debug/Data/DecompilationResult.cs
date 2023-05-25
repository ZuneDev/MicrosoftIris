using System.Xml;

namespace Microsoft.Iris.Debug.Data;

public class DecompilationResult
{
    internal DecompilationResult(string context, XmlDocument doc)
    {
        Context = context;
        Doc = doc;
    }

    public string Context { get; private set; }
    public XmlDocument Doc { get; private set; }
}
