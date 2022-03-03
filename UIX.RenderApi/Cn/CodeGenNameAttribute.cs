// Decompiled with JetBrains decompiler
// Type: Cn.CodeGenNameAttribute
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Diagnostics;

namespace Cn
{
    [Conditional("NEVER")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class CodeGenNameAttribute : Attribute
    {
        public string Name;

        public CodeGenNameAttribute(string stName) => this.Name = stName;
    }
}
