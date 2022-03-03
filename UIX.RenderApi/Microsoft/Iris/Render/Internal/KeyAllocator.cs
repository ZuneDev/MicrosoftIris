// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.KeyAllocator
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System.Threading;

namespace Microsoft.Iris.Render.Internal
{
    internal class KeyAllocator
    {
        private static int s_idxKeyGen;

        internal static uint ReserveSlot() => (uint)Interlocked.Increment(ref s_idxKeyGen);
    }
}
