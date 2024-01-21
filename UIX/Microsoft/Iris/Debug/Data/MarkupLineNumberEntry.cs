using System;

namespace Microsoft.Iris.Debug.Data;

[Serializable]
public record struct MarkupLineNumberEntry(uint Offset, int Line, int Column);
