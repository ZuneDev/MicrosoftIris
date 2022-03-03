// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.HRESULT
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Internal
{
    internal struct HRESULT
    {
        public int hr;

        public HRESULT(int hr) => this.hr = hr;

        public static bool operator ==(HRESULT hrA, HRESULT hrB) => hrA.hr == hrB.hr;

        public static bool operator !=(HRESULT hrA, HRESULT hrB) => hrA.hr != hrB.hr;

        public override bool Equals(object oCompare) => oCompare is HRESULT hresult && this.hr == hresult.hr;

        public override int GetHashCode() => this.hr;

        public bool IsError() => this.hr < 0;

        public bool IsSuccess() => this.hr >= 0;

        public void HandleError()
        {
            if (!this.IsError())
                return;
            Marshal.ThrowExceptionForHR(this.hr);
        }

        public int Int => this.hr;
    }
}
