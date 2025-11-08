using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using GameInputDotNet.Interop.Interfaces;
using Microsoft.Win32.SafeHandles;

namespace GameInputDotNet.Interop.Handles;

[SupportedOSPlatform("windows")]
internal sealed class GameInputRawDeviceReportHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private IGameInputRawDeviceReport? _report;

    private GameInputRawDeviceReportHandle() : base(true)
    {
    }

    public static GameInputRawDeviceReportHandle FromInterface(IGameInputRawDeviceReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        var handle = new GameInputRawDeviceReportHandle
        {
            handle = Marshal.GetIUnknownForObject(report)
        };

        handle._report = report;
        return handle;
    }

    public IGameInputRawDeviceReport GetInterface()
    {
        if (handle == IntPtr.Zero)
        {
            throw new ObjectDisposedException(nameof(GameInputRawDeviceReportHandle), "GameInputRawDeviceReportHandle object can not be disposed.");
        }
        return _report ??= (IGameInputRawDeviceReport)Marshal.GetObjectForIUnknown(handle);
    }

    protected override bool ReleaseHandle()
    {
        Marshal.Release(handle);
        _report = null;
        return true;
    }
}
