// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IDebug
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render
{
    public interface IDebug : IDisposable
    {
        void Enter(string context);

        void Leave(string context);

        void Assert(bool condition, string message);

        void Throw(bool condition, string message);

        void Write(DebugCategory category, int verbosityLevel, string message);

        void WriteLine(DebugCategory category, int verbosityLevel, string message);

        void Indent(DebugCategory category, int verbosityLevel);

        void Unindent(DebugCategory category, int verbosityLevel);

        bool IsCategoryEnabled(DebugCategory category, int verbosityLevel);
    }
}
