// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.Clipboard
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.OS;

namespace Microsoft.Iris.ModelItems
{
    internal static class Clipboard
    {
        public static bool ContainsText()
        {
#if WINDOWS
            return Win32Api.IsClipboardFormatAvailable(13U);
#else
            // TODO: no cross-platform clipboard access is wired up yet (see
            // Microsoft.Iris.Render.Text.Editing.IClipboardAdapter, unused by
            // any platform so far) - report "nothing to paste" rather than
            // crashing on the native check.
            return false;
#endif
        }

        private enum ClipboardFormats
        {
            Text = 13, // 0x0000000D
        }
    }
}
