// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.DllMethodSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using System;
using System.Diagnostics;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class DllMethodSchema : MethodSchema
    {
        private bool _isStatic;
        private TypeSchema _returnType;
        private TypeSchema[] _parameterTypes;
        private string _name;
        private uint _id;

        public DllMethodSchema(DllTypeSchema owner, uint ID)
          : base(owner)
          => _id = ID;

        public bool Load(IntPtr method) => QueryMethodName(method) && QueryForParameterTypes(method) && QueryReturnType(method) && QueryIsStatic(method);

        public uint ID => _id;

        [Conditional("DEBUG")]
        public void DEBUG_Dump()
        {
            string str1 = string.Empty;
            if (IsStatic)
                str1 = "static ";
            string str2 = "void";
            if (ReturnType != null)
                str2 = ReturnType.Name;
            string str3 = string.Empty;
            if (_parameterTypes != null)
            {
                for (int index = 0; index < _parameterTypes.Length; ++index)
                {
                    string str4 = "<null>";
                    if (_parameterTypes[index] != null)
                        str4 = _parameterTypes[index].Name;
                    str3 = string.Format("{0}{1}{2}", str3, index > 0 ? ", " : string.Empty, str4);
                }
            }
            string.Format("0x{0:x8} {1}{2} {3}({4})", _id, str1, str2, Name, str3);
        }

        public override string Name => _name;

        public override TypeSchema[] ParameterTypes => _parameterTypes;

        public override TypeSchema ReturnType => _returnType;

        public override bool IsStatic => _isStatic;

        private DllTypeSchema OwnerTypeSchema => (DllTypeSchema)Owner;

        public override object Invoke(object instance, object[] parameters) => OwnerTypeSchema.InvokeMethod(instance, this, parameters);

        private DllLoadResult OwnerLoadResult => (DllLoadResult)Owner.Owner;

        private unsafe bool QueryMethodName(IntPtr method)
        {
            bool flag = false;
            char* name;
            if (CheckNativeReturn(NativeApi.SpQueryMethodName(method, out name)))
            {
                _name = new string(name);
                flag = true;
            }
            return flag;
        }

        private bool QueryForParameterTypes(IntPtr method)
        {
            bool flag1 = false;
            uint count;
            if (CheckNativeReturn(NativeApi.SpQueryMethodParameterCount(method, out count)))
            {
                if (count > 0U)
                {
                    uint[] IDs = new uint[(int)count];
                    if (CheckNativeReturn(NativeApi.SpGetMethodParameterTypes(method, IDs, count)))
                    {
                        _parameterTypes = new TypeSchema[(int)count];
                        bool flag2 = false;
                        for (uint index = 0; index < count; ++index)
                        {
                            TypeSchema typeSchema = DllLoadResult.MapType(IDs[index]);
                            if (typeSchema != null)
                                _parameterTypes[index] = typeSchema;
                            else
                                flag2 = true;
                        }
                        flag1 = !flag2;
                    }
                }
                else
                {
                    _parameterTypes = TypeSchema.EmptyList;
                    flag1 = true;
                }
            }
            return flag1;
        }

        private bool QueryReturnType(IntPtr method)
        {
            bool flag = false;
            uint type;
            if (CheckNativeReturn(NativeApi.SpQueryMethodReturnType(method, out type)))
            {
                _returnType = DllLoadResult.MapType(type);
                flag = _returnType != null;
            }
            return flag;
        }

        private bool QueryIsStatic(IntPtr method) => CheckNativeReturn(NativeApi.SpQueryMethodIsStatic(method, out _isStatic));

        private bool CheckNativeReturn(uint hr) => DllLoadResult.CheckNativeReturn(hr, "IUIXMethod");
    }
}
