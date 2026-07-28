using System;
using System.Collections.Generic;

namespace Microsoft.Iris.Render.Monitors;

public interface IMonitorSystem
{
    List<MonitorSize> DetectMonitors();
    
    float GetDpi();

    float GetDisplayScale();
}

public static class MonitorSystem
{
    public static IMonitorSystem Instance { get; private set; }
    
    public static void Initialize(IMonitorSystem monitorSystem)
    {
        if (Instance != null)
            throw new Exception($"{nameof(MonitorSystem)} already initialized");
        
        Instance = monitorSystem;
    }
}
