using System.Runtime.InteropServices;

namespace IrisShell.UI;

public static class OSInfo
{
    private const uint SPI_GETKEYBOARDSPEED = 10;
    private const uint SPI_GETKEYBOARDDELAY = 22;
    private static readonly int s_defaultKeyDelay = GetDefaultKeyDelay();
    private static readonly int s_defaultKeyRepeat = GetDefaultKeyRepeat();

    public static int DefaultKeyDelay => s_defaultKeyDelay;

    public static int DefaultKeyRepeat => s_defaultKeyRepeat;

    private static int GetDefaultKeyDelay()
    {
        if (!SystemParametersInfo(SPI_GETKEYBOARDDELAY, 0U, out int pParam, 0))
            pParam = 1;
        return (pParam + 1) * 250;
    }

    private static int GetDefaultKeyRepeat()
    {
        if (!SystemParametersInfo(SPI_GETKEYBOARDSPEED, 0U, out int pParam, 0))
            pParam = 1;
        return 31000 / (62 + 28 * pParam);
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, out int pParam, int nWinIni);
}
