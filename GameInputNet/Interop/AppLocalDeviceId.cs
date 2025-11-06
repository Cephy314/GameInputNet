using System.Runtime.InteropServices;

namespace GameInputNet.Interop;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct AppLocalDeviceId
{
    private fixed byte _value[Constants.APP_LOCAL_DEVICE_ID_SIZE];

    public ReadOnlySpan<byte> AsSpan()
    {
        fixed (byte* ptr = _value)
        {
            return new ReadOnlySpan<byte>(ptr, Constants.APP_LOCAL_DEVICE_ID_SIZE);
        }
    }

    public void CopyTo(Span<byte> destination)
    {
        if (destination.Length < Constants.APP_LOCAL_DEVICE_ID_SIZE)
            throw new ArgumentException(
                $"Destination must be at least {Constants.APP_LOCAL_DEVICE_ID_SIZE} bytes", nameof(destination));

        AsSpan().CopyTo(destination);
    }

    public void Set(ReadOnlySpan<byte> source)
    {
        if (source.Length != Constants.APP_LOCAL_DEVICE_ID_SIZE)
            throw new ArgumentException($"Source must be exactly {Constants.APP_LOCAL_DEVICE_ID_SIZE} bytes",
                nameof(source));

        fixed (byte* ptr = _value)
        {
            source.CopyTo(new Span<byte>(ptr, Constants.APP_LOCAL_DEVICE_ID_SIZE));
        }
    }
}