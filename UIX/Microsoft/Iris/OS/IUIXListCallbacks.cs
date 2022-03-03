// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.IUIXListCallbacks
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.OS
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("0C0FFEE0-06BB-4DCE-8471-D180BF37002D")]
    internal interface IUIXListCallbacks
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        void ListChanged(int type, int oldIndex, int newIndex, int count);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SlowDataAcquireComplete(int index);
    }
}
