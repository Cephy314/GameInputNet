using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using GameInputDotNet.Interop.Interfaces;
using Microsoft.Win32.SafeHandles;

namespace GameInputDotNet.Interop.Handles;

/// <summary>
///     Safe handle managing the lifetime of an <see cref="IGameInputMapper" /> COM pointer.
/// </summary>
[SupportedOSPlatform("windows")]
internal sealed class GameInputMapperHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private IGameInputMapper? _mapper;

    private GameInputMapperHandle()
        : base(true)
    {
    }

    public static GameInputMapperHandle FromInterface(IGameInputMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);

        var handle = new GameInputMapperHandle
        {
            handle = Marshal.GetIUnknownForObject(mapper)
        };

        handle._mapper = mapper;
        return handle;
    }

    public IGameInputMapper GetInterface()
    {
        if (handle == IntPtr.Zero)
        {
            throw new ObjectDisposedException(nameof(GameInputMapperHandle), "GameInputMapperHandle object can not be disposed.");
        }

        return _mapper ??= (IGameInputMapper)Marshal.GetObjectForIUnknown(handle);
    }

    protected override bool ReleaseHandle()
    {
        Marshal.Release(handle);
        _mapper = null;
        return true;
    }
}
