namespace GameInputNet.Interop;

internal static class HResult
{
    public const int S_OK = 0;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Matches Win32 naming.")]
    public static bool SUCCEEDED(int value) => value >= 0;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Matches Win32 naming.")]
    public static bool FAILED(int value) => value < 0;
}
