using System.Runtime.InteropServices;

namespace GameInputNet.Interop;

internal partial struct GameInputHapticInfo
{
    public string GetAudioEndpointId()
    {
        unsafe
        {
            fixed (char* ptr = _audioEndpointId)
            {
                return ptr is null
                    ? string.Empty
                    : Marshal.PtrToStringUni((nint)ptr) ?? string.Empty;
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
            fixed (char* ptr = _audioEndpointId)
            {
                value.CopyTo(new Span<char>(ptr, Constants.GAMEINPUT_HAPTIC_MAX_AUDIO_ENDPOINT_ID_SIZE));
                ptr[value.Length] = '\0';
            }
        }
    }

    public ReadOnlySpan<Guid> GetLocations()
    {
        if (LocationCount == 0 || _locations is null) return ReadOnlySpan<Guid>.Empty;

        return _locations.AsSpan(0, checked((int)LocationCount));
    }
}