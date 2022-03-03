// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.IImeCallbacks
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.RenderAPI;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.OS
{
    [Guid("A851425E-61FE-4D1A-9680-F2617089F403")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IImeCallbacks
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT OnImeMessageReceived(uint msg, UIntPtr wParam, UIntPtr lParam);
    }
}
