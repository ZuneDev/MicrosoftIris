// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.StringProxyHandleTable
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Runtime.InteropServices;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class StringProxyHandleTable : ProxyHandleTable<string>
    {
        private static Map<string, ulong> s_stringToHandleLookup;
        private static Map<ulong, StringPinState> s_pinnedStrings;

        public bool GetStringHandle(string value, out ulong handle)
        {
            if (s_stringToHandleLookup == null)
                s_stringToHandleLookup = new Map<string, ulong>();
            bool flag = s_stringToHandleLookup.TryGetValue(value, out handle);
            if (flag)
            {
                AddRefHandle(handle);
            }
            else
            {
                handle = AllocateHandle(value);
                s_stringToHandleLookup[value] = handle;
            }
            return !flag;
        }

        public bool ReleaseStringHandle(ulong handle, out string value)
        {
            bool flag = ReleaseHandle(handle, out value);
            if (flag)
                s_stringToHandleLookup.Remove(value);
            return flag;
        }

        public bool LookupByHandle(ulong handle, out string obj) => InternalLookupByHandle(handle, out obj);

        public unsafe int PinString(ulong handle, out char* value)
        {
            if (s_pinnedStrings == null)
                s_pinnedStrings = new Map<ulong, StringPinState>();
            StringPinState stringPinState;
            if (s_pinnedStrings.TryGetValue(handle, out stringPinState))
            {
                ++stringPinState._pinCount;
            }
            else
            {
                string str;
                LookupByHandle(handle, out str);
                stringPinState._gcHandle = GCHandle.Alloc(str, GCHandleType.Pinned);
                stringPinState._pinCount = 1;
            }
            s_pinnedStrings[handle] = stringPinState;
            value = (char*)stringPinState._gcHandle.AddrOfPinnedObject().ToPointer();
            return stringPinState._pinCount;
        }

        public int UnpinString(ulong handle)
        {
            StringPinState pinnedString = s_pinnedStrings[handle];
            --pinnedString._pinCount;
            if (pinnedString._pinCount == 0)
            {
                pinnedString._gcHandle.Free();
                s_pinnedStrings.Remove(handle);
            }
            return pinnedString._pinCount;
        }
    }
}
