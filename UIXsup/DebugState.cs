namespace Microsoft.Iris.Support;

internal static class DebugState
{
    private static readonly byte[] s_categoryLevels = new byte[(int)DebugCategory.TotalCount];

    public static bool TimedWriteLines { get; set; }
    public static string WriteLinePrefix { get; set; } = string.Empty;

    public static byte GetCategoryLevel(DebugCategory category)
    {
        int index = (int)category;
        return (uint)index < (uint)s_categoryLevels.Length ? s_categoryLevels[index] : (byte)0;
    }

    public static void SetCategoryLevel(DebugCategory category, byte level)
    {
        int index = (int)category;
        if ((uint)index < (uint)s_categoryLevels.Length)
            s_categoryLevels[index] = level;
    }
}
