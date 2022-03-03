// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.RenderAPI.HRESULT
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.RenderAPI
{
    internal struct HRESULT
    {
        public int hr;

        public HRESULT(int hr) => this.hr = hr;

        public static bool operator ==(HRESULT hrA, HRESULT hrB) => hrA.hr == hrB.hr;

        public static bool operator !=(HRESULT hrA, HRESULT hrB) => hrA.hr != hrB.hr;

        public override bool Equals(object oCompare) => oCompare is HRESULT hresult && hr == hresult.hr;

        public override int GetHashCode() => hr;

        public override string ToString() => "hr:" + hr.ToString("X", CultureInfo.InvariantCulture);

        public bool IsError() => hr < 0;

        public bool IsSuccess() => hr >= 0;

        public void HandleError()
        {
            if (!IsError())
                return;
            Marshal.ThrowExceptionForHR(hr);
        }

        public int Int => hr;
    }
}
