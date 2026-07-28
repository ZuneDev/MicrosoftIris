using System.Collections.Generic;
using Microsoft.Iris.Render.Monitors;
using Silk.NET.GLFW;

namespace Microsoft.Iris.Render.OpenGL.Engine;

public class GlfwMonitorSystem : IMonitorSystem
{
    private readonly Glfw _glfw = Glfw.GetApi();
    
    public unsafe List<MonitorSize> DetectMonitors()
    {
        var monitorHandles = _glfw.GetMonitors(out var count);
        
        var monitors = new List<MonitorSize>(count);
        for (var m = 0; m < count; m++)
        {
            var monitor = monitorHandles[m];
            
            var videoMode = _glfw.GetVideoMode(monitor);
            Rectangle totalRect = new(0, 0, videoMode->Width, videoMode->Height);
            
            _glfw.GetMonitorWorkarea(monitor, out var workX, out var workY, out var workWidth, out var workHeight);
            Rectangle workRect = new(workX, workY, workX + workWidth, workY + workHeight);
            
            monitors.Add(new MonitorSize(totalRect, workRect));
        }

        return monitors;
    }

    public float GetDpi() => GetDisplayScale() * 96.0f;

    public unsafe float GetDisplayScale()
    {
        var monitor = _glfw.GetPrimaryMonitor();
        _glfw.GetMonitorContentScale(monitor, out var xscale, out _);
        return xscale;
    }
}