// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.DllPropertySchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using System;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class DllPropertySchema : PropertySchema
    {
        private BitVector32 _bits;
        private string _name;
        private TypeSchema _type;
        private uint _id;

        public DllPropertySchema(DllTypeSchema owner, uint ID)
          : base(owner)
          => _id = ID;

        public bool Load(IntPtr property) => QueryPropertyName(property) && QueryPropertyType(property) && (QueryCanRead(property) && QueryCanWrite(property)) && QueryIsStatic(property) && QueryNotifiesOnChange(property);

        public uint ID => _id;

        [Conditional("DEBUG")]
        public void DEBUG_Dump()
        {
            string str1 = string.Empty;
            if (IsStatic)
                str1 = "static ";
            string str2 = "<null>";
            if (PropertyType != null)
                str2 = PropertyType.Name;
            string str3 = string.Empty;
            if (CanRead)
                str3 = "get;";
            string str4 = string.Empty;
            if (CanWrite)
                str4 = string.Format("{0}set;", CanRead ? " " : string.Empty);
            string str5 = "<notifies>";
            if (!NotifiesOnChange)
                str5 = "<doesn't notify>";
            string.Format("0x{0:x8} {1}{2} {3} {{{4}{5}}} {6}", _id, str1, str2, Name, str3, str4, str5);
        }

        public override string Name => _name;

        public override TypeSchema PropertyType => _type;

        public override TypeSchema AlternateType => (TypeSchema)null;

        public override bool CanRead => GetBit(Bits.CanRead);

        public override bool CanWrite => GetBit(Bits.CanWrite);

        public override bool IsStatic => GetBit(Bits.IsStatic);

        public override ExpressionRestriction ExpressionRestriction => ExpressionRestriction.None;

        public override bool RequiredForCreation => false;

        public override RangeValidator RangeValidator => (RangeValidator)null;

        public override bool NotifiesOnChange => GetBit(Bits.NotifiesOnChange);

        private DllTypeSchema OwnerTypeSchema => (DllTypeSchema)Owner;

        public override object GetValue(object instance) => OwnerTypeSchema.GetPropertyValue(instance, this);

        public override void SetValue(ref object instance, object value) => OwnerTypeSchema.SetPropertyValue(instance, this, value);

        private DllLoadResult OwnerLoadResult => (DllLoadResult)Owner.Owner;

        private unsafe bool QueryPropertyName(IntPtr property)
        {
            bool flag = false;
            char* name;
            if (CheckNativeReturn(NativeApi.SpQueryPropertyName(property, out name)))
            {
                _name = NotifyService.CanonicalizeString(new string(name));
                flag = true;
            }
            return flag;
        }

        private bool QueryPropertyType(IntPtr property)
        {
            bool flag = false;
            uint type;
            if (CheckNativeReturn(NativeApi.SpQueryPropertyType(property, out type)))
            {
                _type = DllLoadResult.MapType(type);
                flag = true;
            }
            return flag;
        }

        private bool QueryCanRead(IntPtr property)
        {
            bool flag = false;
            bool canRead;
            if (CheckNativeReturn(NativeApi.SpQueryPropertyCanRead(property, out canRead)))
            {
                SetBit(Bits.CanRead, canRead);
                flag = true;
            }
            return flag;
        }

        private bool QueryCanWrite(IntPtr property)
        {
            bool flag = false;
            bool canWrite;
            if (CheckNativeReturn(NativeApi.SpQueryPropertyCanWrite(property, out canWrite)))
            {
                SetBit(Bits.CanWrite, canWrite);
                flag = true;
            }
            return flag;
        }

        private bool QueryIsStatic(IntPtr property)
        {
            bool flag = false;
            bool isStatic;
            if (CheckNativeReturn(NativeApi.SpQueryPropertyIsStatic(property, out isStatic)))
            {
                SetBit(Bits.IsStatic, isStatic);
                flag = true;
            }
            return flag;
        }

        private bool CheckNativeReturn(uint hr) => DllLoadResult.CheckNativeReturn(hr, "IUIXProperty");

        private bool QueryNotifiesOnChange(IntPtr property)
        {
            bool flag = false;
            bool notifiesOnChange;
            if (CheckNativeReturn(NativeApi.SpQueryPropertyNotifiesOnChange(property, out notifiesOnChange)))
            {
                SetBit(Bits.NotifiesOnChange, notifiesOnChange);
                flag = true;
            }
            return flag;
        }

        private bool GetBit(DllPropertySchema.Bits lookupBit) => _bits[(int)lookupBit];

        private void SetBit(DllPropertySchema.Bits changeBit, bool value) => _bits[(int)changeBit] = value;

        private enum Bits : uint
        {
            CanRead = 1,
            CanWrite = 2,
            IsStatic = 4,
            NotifiesOnChange = 8,
        }
    }
}
