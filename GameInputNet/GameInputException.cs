using System.Runtime.InteropServices;

namespace GameInputNet;

/// <summary>
/// Represents an error returned by the native GameInput runtime.
/// </summary>
public sealed class GameInputException : ExternalException
{
    internal GameInputException(string message, int hresult)
        : base($"{message} HRESULT: 0x{hresult:X8}", hresult)
    {
    }

    internal static void ThrowIfFailed(int hresult, string message)
    {
        if (Interop.HResult.SUCCEEDED(hresult))
        {
            return;
        }

        throw new GameInputException(message, hresult);
    }
}
