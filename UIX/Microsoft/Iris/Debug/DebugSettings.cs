using Microsoft.Iris.Debug.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Iris.Debug;

public class DebugSettings
{
    private readonly HashSet<Breakpoint> _breakpoints = [];
    private readonly object _breakpointsLock = new();
    
    public bool UseDecompiler { get; set; } = false;
    public ObservableCollection<DecompilationResult> DecompileResults { get; } = new();

    public TraceSettings TraceSettings { get; } = TraceSettings.Current;

    public bool GenerateDataMappingModels { get; set; } = false;
    public ObservableCollection<DataMappingModel> DataMappingModels { get; } = new();

    public IReadOnlySet<Breakpoint> Breakpoints => _breakpoints;

    public void AddBreakpoint(Breakpoint breakpoint)
    {
        lock (_breakpointsLock)
        {
            _breakpoints.Add(breakpoint);
        }
    }

    public void RemoveBreakpoint(Breakpoint breakpoint)
    {
        lock (_breakpointsLock)
        {
            _breakpoints.Remove(breakpoint);
        }
    }
}
