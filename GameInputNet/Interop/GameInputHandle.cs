

// Interop/GameInputHandle.cs
using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace GameInputNet.Interop;

/// <summary>
/// Safe wrapper over the unmanaged IGameInput COM pointer.
/// </summary>
internal sealed class GameInputHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private GameInputHandle()
        : base(ownsHandle: true)
    {
    }

    public static GameInputHandle FromInterface(GameInputNative.IGameInput gameInput)
    {
        var handle = new GameInputHandle
        {
            handle = Marshal.GetIUnknownForObject(gameInput)
        };

        // GetIUnknownForObject adds a ref; keep the RCW alive so ReleaseHandle
        // decrements the same ref count we just incremented.
        handle._gameInput = gameInput;
        return handle;
    }

    public GameInputNative.IGameInput GetInterface()
    {
        if (IsInvalid)
        {
            throw new ObjectDisposedException(nameof(GameInputHandle));
        }
        if (_gameInput is null)
        {
            _gameInput = (GameInputNative.IGameInput)Marshal.GetObjectForIUnknown(handle);
        }

        return _gameInput;
    }

    protected override bool ReleaseHandle()
    {
        Marshal.Release(handle);
        _gameInput = null;
        return true;
    }

    private GameInputNative.IGameInput? _gameInput;
}
