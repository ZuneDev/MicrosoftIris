// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.HRESULT
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Internal
{
    public struct HRESULT
    {
        internal int hr;

        public HRESULT(int hr) => this.hr = hr;

        public static bool operator ==(HRESULT hrA, HRESULT hrB) => hrA.hr == hrB.hr;

        public static bool operator !=(HRESULT hrA, HRESULT hrB) => hrA.hr != hrB.hr;
        
        public static implicit operator HRESULT(int hr) => new(hr);
        public static implicit operator HRESULT(uint hr) => new(unchecked((int)hr));
        public static implicit operator int(HRESULT hr) => hr.Int;

        public override bool Equals(object oCompare) => oCompare is HRESULT hresult && hr == hresult.hr;

        public override int GetHashCode() => hr;

        public bool IsError() => hr < 0;

        public bool IsSuccess() => hr >= 0;

        public void HandleError()
        {
            if (!IsError())
                return;
            
            Marshal.ThrowExceptionForHR(hr);
        }

        public int Int => hr;
        
        public static HRESULT S_OK => new(0);
        public static HRESULT E_FAIL => 0x80004005;
    }
}
