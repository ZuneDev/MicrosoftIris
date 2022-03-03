// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupDebugHelper
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Collections;
using System.Diagnostics;

namespace Microsoft.Iris.Markup
{
    internal static class MarkupDebugHelper
    {
        [Conditional("ENABLE_STATEDBG")]
        public static void Shutdown()
        {
        }

        public static bool EnterDebugState(
          bool wasInDebugState,
          MarkupLoadResult loadResult,
          int breakpointIndex,
          IDictionary storage)
        {
            return false;
        }

        [Conditional("ENABLE_STATEDBG")]
        public static void LeaveDebugState()
        {
        }
    }
}
