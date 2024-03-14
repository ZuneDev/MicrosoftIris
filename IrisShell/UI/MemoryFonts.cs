using System.Runtime.InteropServices;

namespace IrisShell.UI;

internal class MemoryFonts
{
    public const uint LOAD_LIBRARY_AS_DATAFILE = 2;
    public const int RT_DATA = 10;

    public static bool TryLoadFromResource(string resourceDllName, string fontResourceName)
    {
        IntPtr instanceHandle = IntPtr.Zero;
        try
        {
            instanceHandle = LoadLibraryEx(resourceDllName, IntPtr.Zero, 2U);
            if (instanceHandle == IntPtr.Zero)
                return false;
            IntPtr resource = FindResource(instanceHandle, fontResourceName, (IntPtr)10);
            if (resource == IntPtr.Zero)
                return false;
            IntPtr resourceHandle = LoadResource(instanceHandle, resource);
            return !(resourceHandle == IntPtr.Zero) && !(AddFontMemResourceEx(LockResource(resourceHandle), SizeofResource(instanceHandle, resource), IntPtr.Zero, out uint _) == IntPtr.Zero);
        }
        finally
        {
            if (instanceHandle != IntPtr.Zero)
                FreeLibrary(instanceHandle);
        }
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr LoadLibraryEx(string moduleName, IntPtr reserved, uint flags);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern bool FreeLibrary(IntPtr instanceHandle);

    [DllImport("kernel32.dll", EntryPoint = "FindResourceW", CharSet = CharSet.Unicode)]
    public static extern IntPtr FindResource(
      IntPtr instanceHandle,
      string resource,
      IntPtr type);

    [DllImport("kernel32.dll")]
    public static extern IntPtr LoadResource(IntPtr instanceHandle, IntPtr resourceHandle);

    [DllImport("kernel32.dll")]
    public static extern int SizeofResource(IntPtr instanceHandle, IntPtr resourceHandle);

    [DllImport("kernel32.dll")]
    public static extern IntPtr LockResource(IntPtr resourceHandle);

    [DllImport("gdi32.dll")]
    public static extern IntPtr AddFontMemResourceEx(
      IntPtr fontBuffer,
      int fontButtonSize,
      IntPtr reserved,
      out uint fontsInstalled);
}
