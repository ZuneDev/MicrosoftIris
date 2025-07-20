using Microsoft.Iris.Debug.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Iris.Debug;

public class DebugSettings
{
    public bool UseDecompiler { get; set; } = false;
    public ObservableCollection<DecompilationResult> DecompileResults { get; } = new();

    public TraceSettings TraceSettings { get; } = TraceSettings.Current;

    public bool GenerateDataMappingModels { get; set; } = false;
    public ObservableCollection<DataMappingModel> DataMappingModels { get; } = new();

    public HashSet<Breakpoint> Breakpoints { get; } = new();
}
