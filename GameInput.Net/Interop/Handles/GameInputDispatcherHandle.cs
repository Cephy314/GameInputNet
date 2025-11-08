using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using GameInputDotNet.Interop.Interfaces;
using Microsoft.Win32.SafeHandles;

namespace GameInputDotNet.Interop.Handles;

/// <summary>
///     Safe handle managing the lifetime of an <see cref="IGameInputDispatcher" /> COM pointer.
/// </summary>
[SupportedOSPlatform("windows")]
internal sealed class GameInputDispatcherHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private IGameInputDispatcher? _dispatcher;

    private GameInputDispatcherHandle()
        : base(true)
    {
    }

    public static GameInputDispatcherHandle FromInterface(IGameInputDispatcher dispatcher)
    {
        ArgumentNullException.ThrowIfNull(dispatcher);

        var handle = new GameInputDispatcherHandle
        {
            handle = Marshal.GetIUnknownForObject(dispatcher)
        };

        handle._dispatcher = dispatcher;
        return handle;
    }

    public IGameInputDispatcher GetInterface()
    {
        if (handle == IntPtr.Zero)
        {
            throw new ObjectDisposedException(nameof(GameInputDispatcherHandle), "GameInputDispatcherHandle object can not be disposed.");
        }

        return _dispatcher ??= (IGameInputDispatcher)Marshal.GetObjectForIUnknown(handle);
    }

    protected override bool ReleaseHandle()
    {
        Marshal.Release(handle);
        _dispatcher = null;
        return true;
    }
}
