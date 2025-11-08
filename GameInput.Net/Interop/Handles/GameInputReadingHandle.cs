// Interop/GameInputHandle.cs

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using GameInputDotNet.Interop.Interfaces;
using Microsoft.Win32.SafeHandles;

namespace GameInputDotNet.Interop.Handles;

/// <summary>
///     Safe wrapper over the unmanaged IGameInput COM pointer.
/// </summary>
[SupportedOSPlatform("windows")]
internal sealed class GameInputReadingHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private IGameInputReading? _gameInputReading;

    private GameInputReadingHandle()
        : base(true)
    {
    }

    public static GameInputReadingHandle FromInterface(IGameInputReading gameInputReading)
    {
        var handle = new GameInputReadingHandle
        {
            handle = Marshal.GetIUnknownForObject(gameInputReading)
        };

        // GetIUnknownForObject adds a ref; keep the RCW alive so ReleaseHandle
        // decrements the same ref count we just incremented.
        handle._gameInputReading = gameInputReading;
        return handle;
    }

    public IGameInputReading GetInterface()
    {
        if (handle == IntPtr.Zero)
        {
            throw new ObjectDisposedException(nameof(GameInputReadingHandle), "GameInputReadingHandle object can not be disposed.");
        }

        return _gameInputReading ??= (IGameInputReading)Marshal.GetObjectForIUnknown(handle);
    }

    protected override bool ReleaseHandle()
    {
        Marshal.Release(handle);
        _gameInputReading = null;
        return true;
    }
}
