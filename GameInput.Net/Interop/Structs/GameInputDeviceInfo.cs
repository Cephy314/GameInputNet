using System.Runtime.InteropServices;
using System.Text;
using GameInputDotNet.Interop.Enums;

namespace GameInputDotNet.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct GameInputDeviceInfo
{
    public ushort VendorId;
    public ushort ProductId;
    public ushort RevisionNumber;
    public GameInputUsage Usage;
    public GameInputVersion HardwareVersion;
    public GameInputVersion FirmwareVersion;
    public AppLocalDeviceId DeviceId;
    public AppLocalDeviceId DeviceRootId;
    public GameInputDeviceFamily DeviceFamily;
    public GameInputKind SupportedInput;
    public GameInputRumbleMotors SupportedRumbleMotors;
    public GameInputSystemButtons SupportedSystemButtons;
    public Guid ContainerId;

    private const int HidPMaxLength = 4092; // Maximum size assumed for HidD_*String functions

    private unsafe sbyte* DisplayName;
    private unsafe sbyte* PnpPath;

    private unsafe GameInputKeyboardInfo* KeyboardInfo;
    private unsafe GameInputMouseInfo* MouseInfo;
    private unsafe GameInputSensorsInfo* SensorsInfo;
    private unsafe GameInputControllerInfo* ControllerInfo;
    private unsafe GameInputArcadeStickInfo* ArcadeStickInfo;
    private unsafe GameInputFlightStickInfo* FlightStickInfo;
    private unsafe GameInputGamepadInfo* GamepadInfo;
    private unsafe GameInputRacingWheelInfo* RacingWheelInfo;

    public uint ForceFeedbackMotorCount;
    private unsafe GameInputForceFeedbackMotorInfo* ForceFeedbackMotorInfo;

    public uint InputReportCount;
    private unsafe GameInputRawDeviceReportInfo* InputReportInfo;

    public uint OutputReportCount;
    private unsafe GameInputRawDeviceReportInfo* OutputReportInfo;

    public string? GetDisplayName()
    {
        unsafe
        {
            return GetUnsafeUtf8String(DisplayName, HidPMaxLength);
        }
    }

    public string? GetPnpPath()
    {
        unsafe
        {
            return GetUnsafeUtf8String(PnpPath, HidPMaxLength);
        }
    }

    public GameInputKeyboardInfo? GetKeyboardInfo()
    {
        unsafe
        {
            return KeyboardInfo is null ? null : *KeyboardInfo;
        }
    }

    public GameInputMouseInfo? GetMouseInfo()
    {
        unsafe
        {
            return MouseInfo is null ? null : *MouseInfo;
        }
    }

    public GameInputSensorsInfo? GetSensorsInfo()
    {
        unsafe
        {
            return SensorsInfo is null ? null : *SensorsInfo;
        }
    }

    public GameInputControllerInfo? GetControllerInfo()
    {
        unsafe
        {
            return ControllerInfo is null ? null : *ControllerInfo;
        }
    }

    public GameInputArcadeStickInfo? GetArcadeStickInfo()
    {
        unsafe
        {
            return ArcadeStickInfo is null ? null : *ArcadeStickInfo;
        }
    }

    public GameInputFlightStickInfo? GetFlightStickInfo()
    {
        unsafe
        {
            return FlightStickInfo is null ? null : *FlightStickInfo;
        }
    }

    public GameInputGamepadInfo? GetGamepadInfo()
    {
        unsafe
        {
            return GamepadInfo is null ? null : *GamepadInfo;
        }
    }

    public GameInputRacingWheelInfo? GetRacingWheelInfo()
    {
        unsafe
        {
            return RacingWheelInfo is null ? null : *RacingWheelInfo;
        }
    }

    public ReadOnlySpan<GameInputForceFeedbackMotorInfo> GetForceFeedbackMotorInfo()
    {
        if (ForceFeedbackMotorCount == 0)
        {
            return ReadOnlySpan<GameInputForceFeedbackMotorInfo>.Empty;
        }

        unsafe
        {
            return ForceFeedbackMotorInfo is null
                ? ReadOnlySpan<GameInputForceFeedbackMotorInfo>.Empty
                : new ReadOnlySpan<GameInputForceFeedbackMotorInfo>(
                    ForceFeedbackMotorInfo,
                    checked((int)ForceFeedbackMotorCount));
        }
    }

    public ReadOnlySpan<GameInputRawDeviceReportInfo> GetInputReportInfo()
    {
        if (InputReportCount == 0)
        {
            return ReadOnlySpan<GameInputRawDeviceReportInfo>.Empty;
        }

        unsafe
        {
            return InputReportInfo is null
                ? ReadOnlySpan<GameInputRawDeviceReportInfo>.Empty
                : new ReadOnlySpan<GameInputRawDeviceReportInfo>(InputReportInfo, checked((int)InputReportCount));
        }
    }

    public ReadOnlySpan<GameInputRawDeviceReportInfo> GetOutputReportInfo()
    {
        if (OutputReportCount == 0)
        {
            return ReadOnlySpan<GameInputRawDeviceReportInfo>.Empty;
        }

        unsafe
        {
            return OutputReportInfo is null
                ? ReadOnlySpan<GameInputRawDeviceReportInfo>.Empty
                : new ReadOnlySpan<GameInputRawDeviceReportInfo>(OutputReportInfo, checked((int)OutputReportCount));
        }
    }

    /// <summary>
    ///     Return UTF8 String from pointer.
    /// </summary>
    /// <param name="pString">Pointer to the UTF-8 encoded, null-terminated string buffer.</param>
    /// <param name="maxLength">Maximum number of bytes to read from the buffer starting at <paramref name="pString" />.</param>
    /// <returns>String at pointer or null.</returns>
    private static unsafe string? GetUnsafeUtf8String(sbyte* pString, int maxLength)
    {
        if (pString == null)
        {
            return null;
        }

        var length = 0;
        var bytesPtr = (byte*)pString;

        // Traverse bytes to find null terminator. Do not overrun max value.
        // No guarantee we have a buffer of max size.
        while (length < maxLength && bytesPtr[length] != 0)
            length++;

        if (length == 0 || (length == maxLength && bytesPtr[length - 1] != 0))
        {
            return null;
        }

        return Encoding.UTF8.GetString(bytesPtr, length);
    }
}