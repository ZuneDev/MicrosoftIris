// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ThreadSafety
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.Threading;

namespace Microsoft.Iris
{
    public static class ThreadSafety
    {
        public static void InitializeObject(IThreadSafeObject safe)
        {
            if (safe == null)
                throw new ArgumentNullException(nameof(safe));
            safe.Affinity = Thread.CurrentThread;
        }
    }
}
