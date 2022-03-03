// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.DebugNone
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render.Extensions
{
    public class DebugNone : IDebug, IDisposable
    {
        void IDisposable.Dispose()
        {
        }

        void IDebug.Enter(string stContext)
        {
        }

        void IDebug.Leave(string stContext)
        {
        }

        void IDebug.Assert(bool fCondition, string stMessage)
        {
        }

        void IDebug.Throw(bool fCondition, string stMessage)
        {
        }

        void IDebug.Write(DebugCategory nCategory, int nLevel, string stMessage)
        {
        }

        void IDebug.WriteLine(DebugCategory nCategory, int nLevel, string stMessage)
        {
        }

        void IDebug.Indent(DebugCategory nCategory, int nLevel)
        {
        }

        void IDebug.Unindent(DebugCategory nCategory, int nLevel)
        {
        }

        bool IDebug.IsCategoryEnabled(DebugCategory category, int nLevel) => false;
    }
}
