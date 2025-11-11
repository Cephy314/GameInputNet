namespace GameInputDotNet.Interop;

internal static class Constants
{
    public const int APP_LOCAL_DEVICE_ID_SIZE = 32; // match GameInput.h
    public const int GAMEINPUT_HAPTIC_MAX_LOCATIONS = 8;
    public const int GAMEINPUT_HAPTIC_MAX_AUDIO_ENDPOINT_ID_SIZE = 256;
    public const int GAMEINPUT_MAX_SWITCH_STATES = 8;

    public static readonly Guid GAMEINPUT_HAPTIC_LOCATION_NONE =
        new(0x00000000, 0x0000, 0x0000, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00);

    public static readonly Guid GAMEINPUT_HAPTIC_LOCATION_GRIP_LEFT =
        new(0x08c707c2, 0x66bb, 0x406c, 0xa8, 0x4a, 0xdf, 0xe0, 0x85, 0x12,
            0x0a, 0x92);

    public static readonly Guid GAMEINPUT_HAPTIC_LOCATION_GRIP_RIGHT =
        new(0x155a0b77, 0x8bb2, 0x40db, 0x86, 0x90, 0xb6, 0xd4, 0x11, 0x26,
            0xdf, 0xc1);

    public static readonly Guid GAMEINPUT_HAPTIC_LOCATION_TRIGGER_LEFT =
        new(0x8de4d896, 0x5559, 0x4081, 0x86, 0xe5, 0x17, 0x24, 0xcc, 0x07,
            0xc6, 0xbc);

    public static readonly Guid GAMEINPUT_HAPTIC_LOCATION_TRIGGER_RIGHT =
        new(0xff0cb557, 0x3af5, 0x406b, 0x8b, 0x0f, 0x55, 0x5a, 0x2d, 0x92,
            0xa2, 0x20);
}