using System.Runtime.InteropServices;
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

    // @TODO If documentation confirms UTF-8, swap to Encoding.UTF8.GetString.
    public string? GetDisplayName()
    {
        unsafe
        {
            return DisplayName is null
                ? null
                : Marshal.PtrToStringAnsi((nint)DisplayName);
        }
    }

    public string? GetPnpPath()
    {
        unsafe
        {
            return PnpPath is null
                ? null
                : Marshal.PtrToStringAnsi((nint)PnpPath);
        }
    }

    public GameInputKeyboardInfo? GetKeyboardInfo()
    {
        unsafe
        {
            return KeyboardInfo is null
                ? null
                : *KeyboardInfo;
        }
    }

    public GameInputMouseInfo? GetMouseInfo()
    {
        unsafe
        {
            return MouseInfo is null
                ? null
                : *MouseInfo;
        }
    }

    public GameInputSensorsInfo? GetSensorsInfo()
    {
        unsafe
        {
            return SensorsInfo is null
                ? null
                : *SensorsInfo;
        }
    }

    public GameInputControllerInfo? GetControllerInfo()
    {
        unsafe
        {
            return ControllerInfo is null
                ? null
                : *ControllerInfo;
        }
    }

    public GameInputArcadeStickInfo? GetArcadeStickInfo()
    {
        unsafe
        {
            return ArcadeStickInfo is null
                ? null
                : *ArcadeStickInfo;
        }
    }

    public GameInputFlightStickInfo? GetFlightStickInfo()
    {
        unsafe
        {
            return FlightStickInfo is null
                ? null
                : *FlightStickInfo;
        }
    }

    public GameInputGamepadInfo? GetGamepadInfo()
    {
        unsafe
        {
            return GamepadInfo is null
                ? null
                : *GamepadInfo;
        }
    }

    public GameInputRacingWheelInfo? GetRacingWheelInfo()
    {
        unsafe
        {
            return RacingWheelInfo is null
                ? null
                : *RacingWheelInfo;
        }
    }

    public ReadOnlySpan<GameInputForceFeedbackMotorInfo>
        GetForceFeedbackMotorInfo()
    {
        if (ForceFeedbackMotorCount == 0)
            return ReadOnlySpan<GameInputForceFeedbackMotorInfo>.Empty;

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
            return ReadOnlySpan<GameInputRawDeviceReportInfo>.Empty;

        unsafe
        {
            return InputReportInfo is null
                ? ReadOnlySpan<GameInputRawDeviceReportInfo>.Empty
                : new ReadOnlySpan<GameInputRawDeviceReportInfo>(
                    InputReportInfo,
                    checked((int)InputReportCount));
        }
    }

    public ReadOnlySpan<GameInputRawDeviceReportInfo> GetOutputReportInfo()
    {
        if (OutputReportCount == 0)
            return ReadOnlySpan<GameInputRawDeviceReportInfo>.Empty;

        unsafe
        {
            return OutputReportInfo is null
                ? ReadOnlySpan<GameInputRawDeviceReportInfo>.Empty
                : new ReadOnlySpan<GameInputRawDeviceReportInfo>(
                    OutputReportInfo,
                    checked((int)OutputReportCount));
        }
    }
}