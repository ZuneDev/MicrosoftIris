// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.DllEnumSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using Microsoft.Iris.Session;
using System;
using System.Diagnostics;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class DllEnumSchema : EnumSchema
    {
        private uint _id;
        private IntPtr _nativeSchema;

        public DllEnumSchema(LoadResult owner, uint id, IntPtr nativeSchema)
          : base(owner)
        {
            _id = id;
            _nativeSchema = nativeSchema;
        }

        [Conditional("DEBUG")]
        public void DEBUG_DumpEnum()
        {
            int val1 = 0;
            foreach (KeyValueEntry<string, int> nameToValue in NameToValueMap)
            {
                if (nameToValue.Key != null)
                    val1 = Math.Max(val1, nameToValue.Key.Length);
            }
            string.Format("{{0,{0}}} = 0x{{1:x8}}", val1);
            foreach (KeyValueEntry<string, int> nameToValue in NameToValueMap)
                ;
        }

        public uint ID => _id;

        public bool Load()
        {
            string name;
            bool flag = QueryName(out name);
            if (flag)
            {
                bool isFlags;
                flag = QueryIsFlags(out isFlags);
                if (flag)
                {
                    string[] names;
                    int[] values;
                    flag = QueryNamesAndValues(out names, out values);
                    if (flag)
                    {
                        Initialize(name, typeof(DllEnumProxy), isFlags, names, values);
                        flag = true;
                    }
                }
            }
            return flag;
        }

        private unsafe bool QueryName(out string name)
        {
            char* name1;
            bool flag = CheckNativeReturn(NativeApi.SpQueryEnumName(_nativeSchema, out name1));
            name = !flag ? null : new string(name1);
            return flag;
        }

        private bool QueryIsFlags(out bool isFlags) => CheckNativeReturn(NativeApi.SpQueryEnumIsFlags(_nativeSchema, out isFlags));

        private unsafe bool QueryNamesAndValues(out string[] names, out int[] values)
        {
            bool flag1 = false;
            names = null;
            values = null;
            uint valueCount;
            if (CheckNativeReturn(NativeApi.SpQueryEnumValueCount(_nativeSchema, out valueCount)))
            {
                bool flag2 = false;
                if (valueCount > 0U)
                {
                    names = new string[valueCount];
                    values = new int[valueCount];
                    for (uint index = 0; index < valueCount; ++index)
                    {
                        char* name;
                        int num;
                        if (CheckNativeReturn(NativeApi.SpGetEnumNameValue(_nativeSchema, index, out name, out num)))
                        {
                            if ((IntPtr)name != IntPtr.Zero)
                            {
                                names[index] = new string(name);
                                values[index] = num;
                            }
                            else
                                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "Name");
                        }
                        else
                        {
                            flag2 = true;
                            break;
                        }
                    }
                }
                flag1 = !flag2;
            }
            return flag1;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            NativeApi.SpReleaseExternalObject(_nativeSchema);
            _nativeSchema = IntPtr.Zero;
        }

        public object GetBoxedValue(int value) => EnumValueToObject(value);

        protected override object EnumValueToObject(int value) => new DllEnumProxy(this, value);

        protected override int ValueFromObject(object obj) => ((DllEnumProxy)obj).Value;

        public string InvokeToString(DllEnumProxy proxy)
        {
            string str = null;
            IntPtr result;
            if (CheckNativeReturn(NativeApi.SpInvokeEnumToString(_nativeSchema, proxy.Value, out result)))
                str = DllProxyServices.GetString(result);
            return str;
        }

        private DllLoadResult OwnerLoadResult => (DllLoadResult)Owner;

        private bool CheckNativeReturn(uint hr) => DllLoadResult.CheckNativeReturn(hr, "IUIXEnum");
    }
}
