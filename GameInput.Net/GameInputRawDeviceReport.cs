using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using GameInputDotNet.Interop.Handles;
using GameInputDotNet.Interop.Interfaces;
using GameInputDotNet.Interop.Structs;

namespace GameInputDotNet;

[SupportedOSPlatform("windows")]
public sealed class GameInputRawDeviceReport : IDisposable
{
    private GameInputRawDeviceReportHandle? _handle;

    internal GameInputRawDeviceReport(GameInputRawDeviceReportHandle handle)
    {
        ArgumentNullException.ThrowIfNull(handle);
        _handle = handle;
    }

    internal IGameInputRawDeviceReport NativeInterface =>
        _handle?.GetInterface() ?? throw new ObjectDisposedException(nameof(GameInputRawDeviceReport));

    public void Dispose()
    {
        _handle?.Dispose();
        _handle = null;
        GC.SuppressFinalize(this);
    }

    public GameInputDevice GetDevice()
    {
        NativeInterface.GetDevice(out var device);
        if (device is null)
        {
            throw new GameInputException("IGameInputRawDeviceReport.GetDevice returned null.", unchecked((int)0x80004003));
        }

        var status = device.GetDeviceStatus();
        var handle = GameInputDeviceHandle.FromInterface(device);
        return new GameInputDevice(handle, 0, status, status);
    }

    public GameInputRawDeviceReportInfo GetReportInfo()
    {
        NativeInterface.GetReportInfo(out var info);
        return info;
    }

    public byte[] GetRawData()
    {
        var size = NativeInterface.GetRawDataSize();
        if (size == 0) return Array.Empty<byte>();

        var buffer = new byte[size];
        unsafe
        {
            fixed (byte* ptr = buffer)
            {
                NativeInterface.GetRawData(size, ptr);
            }
        }

        return buffer;
    }

    public void SetRawData(ReadOnlySpan<byte> data)
    {
        unsafe
        {
            fixed (byte* ptr = data)
            {
                var result = NativeInterface.SetRawData((nuint)data.Length, ptr);
                if (!result)
                {
                    throw new GameInputException("IGameInputRawDeviceReport.SetRawData failed.", unchecked((int)0x80004005));
                }
            }
        }
    }
}
