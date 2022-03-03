// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.DllEventSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using System;
using System.Diagnostics;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class DllEventSchema : EventSchema
    {
        private string _name;
        private bool _isStatic;

        public DllEventSchema(DllTypeSchema owner, uint ID)
          : base(owner)
        {
        }

        public bool Load(IntPtr eventSchema) => QueryEventName(eventSchema) && QueryIsStatic(eventSchema);

        [Conditional("DEBUG")]
        public void DEBUG_Dump()
        {
        }

        public override string Name => _name;

        public bool IsStatic => _isStatic;

        private unsafe bool QueryEventName(IntPtr eventSchema)
        {
            bool flag = false;
            char* name;
            if (CheckNativeReturn(NativeApi.SpQueryEventName(eventSchema, out name)))
            {
                _name = NotifyService.CanonicalizeString(new string(name));
                flag = true;
            }
            return flag;
        }

        private bool QueryIsStatic(IntPtr eventSchema) => CheckNativeReturn(NativeApi.SpQueryEventIsStatic(eventSchema, out _isStatic));

        private bool CheckNativeReturn(uint hr) => DllLoadResult.CheckNativeReturn(hr, "IUIXEvent");
    }
}
