// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Error
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris
{
    public class Error
    {
        public string Context;
        public string Message;
        public bool Warning;
        public int Line;
        public int Column;

        public override string ToString()
        {
            string str1 = Context;
            if (str1 != null && str1.StartsWith("file://", StringComparison.Ordinal))
                str1 = str1.Substring(7);
            string str2;
            if (Line != -1)
                str2 = string.Format("{0}({1},{2}) : {3} : {4}", str1, Line, Column, Warning ? "warning" : "error", Message);
            else
                str2 = Context == null ? string.Format("{0} : {1}", Warning ? "warning" : "error", Message) : string.Format("{0} : {1} : {2}", str1, Warning ? "warning" : "error", Message);
            return str2;
        }
    }
}
