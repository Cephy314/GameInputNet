using System.Runtime.InteropServices;

namespace GameInputDotNet.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct GameInputHapticInfo
{
    private fixed ushort _audioEndpointId[Constants.GAMEINPUT_HAPTIC_MAX_AUDIO_ENDPOINT_ID_SIZE];
    public uint LocationCount;
    private fixed byte _locations[Constants.GAMEINPUT_HAPTIC_MAX_LOCATIONS * 16];
}

public partial struct GameInputHapticInfo
{
    public string GetAudioEndpointId()
    {
        unsafe
        {
            fixed (ushort* ptr = _audioEndpointId)
            {
                return Marshal.PtrToStringUni((nint)ptr) ?? string.Empty;
            }
        }
    }

    public void SetAudioEndpointId(ReadOnlySpan<char> value)
    {
        if (value.Length >= Constants.GAMEINPUT_HAPTIC_MAX_AUDIO_ENDPOINT_ID_SIZE)
            throw new ArgumentException(
                $"Audio endpoint ID must be shorter than {Constants.GAMEINPUT_HAPTIC_MAX_AUDIO_ENDPOINT_ID_SIZE} characters.",
                nameof(value));

        unsafe
        {
            fixed (ushort* ptr = _audioEndpointId)
            {
                var charPtr = (char*)ptr;
                var buffer = new Span<char>(charPtr, Constants.GAMEINPUT_HAPTIC_MAX_AUDIO_ENDPOINT_ID_SIZE);
                value.CopyTo(buffer);
                buffer[value.Length] = '\0';
            }
        }
    }

    public ReadOnlySpan<Guid> GetLocations()
    {
        if (LocationCount == 0) return ReadOnlySpan<Guid>.Empty;

        unsafe
        {
            fixed (byte* ptr = _locations)
            {
                var count = (int)Math.Min(LocationCount, Constants.GAMEINPUT_HAPTIC_MAX_LOCATIONS);
                var raw = new ReadOnlySpan<byte>(ptr, Constants.GAMEINPUT_HAPTIC_MAX_LOCATIONS * 16);
                return MemoryMarshal.Cast<byte, Guid>(raw).Slice(0, count);
            }
        }
    }

    public void SetLocation(int index, Guid value)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        if (index >= Constants.GAMEINPUT_HAPTIC_MAX_LOCATIONS) throw new ArgumentOutOfRangeException(nameof(index));

        unsafe
        {
            fixed (byte* ptr = _locations)
            {
                var span = MemoryMarshal.Cast<byte, Guid>(
                    new Span<byte>(ptr, Constants.GAMEINPUT_HAPTIC_MAX_LOCATIONS * 16));
                span[index] = value;
            }
        }
    }
}
