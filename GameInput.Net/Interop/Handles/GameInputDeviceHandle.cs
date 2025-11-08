using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using GameInputDotNet.Interop.Interfaces;
using Microsoft.Win32.SafeHandles;

namespace GameInputDotNet.Interop.Handles;

/// <summary>
///     Safe handle managing the lifetime of an <see cref="IGameInputDevice" /> COM pointer.
/// </summary>
[SupportedOSPlatform("windows")]
internal sealed class GameInputDeviceHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private IGameInputDevice? _device;

    private GameInputDeviceHandle()
        : base(true)
    {
    }

    public static GameInputDeviceHandle FromInterface(IGameInputDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);

        var handle = new GameInputDeviceHandle
        {
            handle = Marshal.GetIUnknownForObject(device)
        };

        // Marshal.GetIUnknownForObject adds a ref; retain the RCW so we release the same ref later.
        handle._device = device;
        return handle;
    }

    public IGameInputDevice GetInterface()
    {
        if (handle == IntPtr.Zero)
        {
            throw new ObjectDisposedException(nameof(GameInputDeviceHandle), "GameInputDeviceHandle object can not be disposed.");
        }

        return _device ??= (IGameInputDevice)Marshal.GetObjectForIUnknown(handle);
    }

    protected override bool ReleaseHandle()
    {
        Marshal.Release(handle);
        _device = null;
        return true;
    }
}
