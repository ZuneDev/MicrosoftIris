using System.Collections.Generic;

namespace Microsoft.Iris.Data;

public interface IEnumerableResourceProvider
{
    IEnumerable<Resource> EnumerateResources(string baseHierarchicalPart, string baseUri, bool forceSynchronous);
}