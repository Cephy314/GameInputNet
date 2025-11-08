using System;
using System.Runtime.InteropServices;

namespace GameInputDotNet;

/// <summary>
///     Represents an error returned by the native GameInput runtime.
/// </summary>
public class GameInputException : ExternalException
{
    internal GameInputException(string message, int hresult)
        : base($"{message} HRESULT: 0x{hresult:X8}", hresult)
    {
    }

    /// <summary>
    ///     Gets the strongly typed GameInput error code when the HRESULT maps to a known value; otherwise <c>null</c>.
    /// </summary>
    public GameInputErrorCode? Error
    {
        get
        {
            var candidate = (GameInputErrorCode)ErrorCode;
            return Enum.IsDefined(typeof(GameInputErrorCode), candidate) ? candidate : null;
        }
    }

    internal static void ThrowIfFailed(int hresult, string message)
    {
        if (Interop.HResult.SUCCEEDED(hresult)) return;

        throw new GameInputException(message, hresult);
    }
}
