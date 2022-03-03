// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Library.KeyAllocator
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Threading;

namespace Microsoft.Iris.Library
{
    internal class KeyAllocator
    {
        private static int s_idxKeyGen;

        internal static uint ReserveSlot() => (uint)Interlocked.Increment(ref s_idxKeyGen);
    }
}
