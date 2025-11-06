using System.Runtime.InteropServices;

// ReSharper disable ConvertToAutoProperty

namespace GameInputNet.Interop;

[StructLayout(LayoutKind.Sequential)]
public struct GameInputKeyState
{
    public uint ScanCode;
    public uint CodePoint;
    public byte VirtualKey;

    [MarshalAs(UnmanagedType.I1)] public bool IsDeadKey;
}

[StructLayout(LayoutKind.Sequential)]
public struct GameInputMouseState
{
    public GameInputMouseButtons Buttons;
    public GameInputMousePositions Positions;
    public long PositionX;
    public long PositionY;
    public long AbsolutePositionX;
    public long AbsolutePositionY;
    public long WheelX;
    public long WheelY;
}

[StructLayout(LayoutKind.Sequential)]
public struct GameInputSensorsState
{
    // GameInputSensorsAccelerometer
    public float AccelerationInGx;
    public float AccelerationInGy;
    public float AccelerationInGz;

    // GameInputSensorsGyrometer
    public float AngularVelocityInRadPerSecX;
    public float AngularVelocityInRadPerSecY;
    public float AngularVelocityInRadPerSecZ;

    // GameInputSensorsCompass
    public float HeadingInDegreesFromMagneticNorth;
    public GameInputSensorAccuracy HeadingAccuracy;

    // GameInputSensorsOrientation
    public float OrientationW;
    public float OrientationX;
    public float OrientationY;
    public float OrientationZ;
}

[StructLayout(LayoutKind.Sequential)]
public struct GameInputArcadeStickState
{
    public GameInputArcadeStickButtons Buttons;
}

[StructLayout(LayoutKind.Sequential)]
public struct GameInputFlightStickState
{
    public GameInputFlightStickButtons Buttons;
    public GameInputSwitchPosition HatSwitch;
    public float Roll;
    public float Pitch;
    public float Yaw;
    public float Throttle;
}

[StructLayout(LayoutKind.Sequential)]
public struct GameInputVersion
{
    public ushort Major;
    public ushort Minor;
    public ushort Build;
    public ushort Revision;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputGamepadState
{
    public GameInputGamepadButtons Buttons;
    public float LeftTrigger;
    public float RightTrigger;
    public float LeftThumbstickX;
    public float LeftThumbstickY;
    public float RightThumbstickX;
    public float RightThumbstickY;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputRacingWheelState
{
    public GameInputRacingWheelButtons Buttons;
    public int PatternShifterGear;
    public float Wheel;
    public float Throttle;
    public float Brake;
    public float Clutch;
    public float Handbrake;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputControllerSwitchInfo
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.GAMEINPUT_MAX_SWITCH_STATES)]
    public GameInputLabel[] Labels;

    public GameInputSwitchKind Kind;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe partial struct GameInputControllerInfo
{
    private uint _controllerAxisCount;
    private GameInputLabel* _controllerAxisLabels;
    private uint _controllerButtonCount;
    private GameInputLabel* _controllerButtonLabels;
    private uint _controllerSwitchCount;
    private GameInputControllerSwitchInfo* _controllerSwitchInfo;
}

internal partial struct GameInputControllerInfo
{
    public uint GetControllerAxisCount()
    {
        return _controllerAxisCount;
    }

    public uint GetControllerButtonCount()
    {
        return _controllerButtonCount;
    }

    public uint GetControllerSwitchCount()
    {
        return _controllerSwitchCount;
    }


    public ReadOnlySpan<GameInputLabel> GetControllerAxisLabels()
    {
        if (_controllerAxisCount == 0) return ReadOnlySpan<GameInputLabel>.Empty;
        unsafe
        {
            return _controllerAxisLabels is null
                ? ReadOnlySpan<GameInputLabel>.Empty
                : new ReadOnlySpan<GameInputLabel>(_controllerAxisLabels, checked((int)_controllerAxisCount));
        }
    }

    public ReadOnlySpan<GameInputLabel> GetControllerButtonLabels()
    {
        if (_controllerButtonCount == 0) return ReadOnlySpan<GameInputLabel>.Empty;
        unsafe
        {
            return _controllerButtonLabels is null
                ? ReadOnlySpan<GameInputLabel>.Empty
                : new ReadOnlySpan<GameInputLabel>(_controllerButtonLabels, checked((int)_controllerButtonCount));
        }
    }

    public ReadOnlySpan<GameInputControllerSwitchInfo> GetControllerSwitchInfo()
    {
        if (_controllerSwitchCount == 0) return ReadOnlySpan<GameInputControllerSwitchInfo>.Empty;
        unsafe
        {
            return _controllerSwitchInfo is null
                ? ReadOnlySpan<GameInputControllerSwitchInfo>.Empty
                : new ReadOnlySpan<GameInputControllerSwitchInfo>(_controllerSwitchInfo,
                    checked((int)_controllerSwitchCount));
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputKeyboardInfo
{
    public GameInputKeyboardKind Kind;
    public uint Layout;
    public uint KeyCount;
    public uint functionKeyCount;
    public uint MaxSimultaneousKeys;
    public uint platformType;
    public uint platformSubType;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputMouseInfo
{
    public GameInputMouseButtons SupportedButtons;
    public uint SampleRate;
    public bool HasWheelX;
    public bool HasWheelY;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputSensorsInfo
{
    public GameInputSensorsKind SupportedSensors;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputArcadeStickInfo
{
    public GameInputLabel MenuButtonLabel;
    public GameInputLabel ViewButtonLabel;
    public GameInputLabel StickUpLabel;
    public GameInputLabel StickDownLabel;
    public GameInputLabel StickLeftLabel;
    public GameInputLabel StickRightLabel;
    public GameInputLabel ActionButton1Label;
    public GameInputLabel ActionButton2Label;
    public GameInputLabel ActionButton3Label;
    public GameInputLabel ActionButton4Label;
    public GameInputLabel ActionButton5Label;
    public GameInputLabel ActionButton6Label;
    public GameInputLabel SpecialButton1Label;
    public GameInputLabel SpecialButton2Label;
    public uint ExtraButtonCount;
    public uint ExtraAxisCount;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputFlightStickInfo
{
    public GameInputLabel MenuButtonLabel;
    public GameInputLabel ViewButtonLabel;
    public GameInputLabel FirePrimaryButtonLabel;
    public GameInputLabel FireSecondaryButtonLabel;
    public GameInputLabel HatSwitchUpLabel;
    public GameInputLabel HatSwitchDownLabel;
    public GameInputLabel HatSwitchLeftLabel;
    public GameInputLabel HatSwitchRightLabel;
    public GameInputLabel AButtonLabel;
    public GameInputLabel BButtonLabel;
    public GameInputLabel XButtonLabel;
    public GameInputLabel YButtonLabel;
    public GameInputLabel LeftShoulderButtonLabel;
    public GameInputLabel RightShoulderButtonLabel;
    public uint extraButtonCount;
    public uint extraAxisCount;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputGamepadInfo
{
    public GameInputGamepadButtons SupportedLayout;
    public GameInputLabel MenuButtonLabel;
    public GameInputLabel ViewButtonLabel;
    public GameInputLabel AButtonLabel;
    public GameInputLabel BButtonLabel;
    public GameInputLabel CButtonLabel;
    public GameInputLabel XButtonLabel;
    public GameInputLabel YButtonLabel;
    public GameInputLabel ZButtonLabel;
    public GameInputLabel DpadUpLabel;
    public GameInputLabel DpadDownLabel;
    public GameInputLabel DpadLeftLabel;
    public GameInputLabel DpadRightLabel;
    public GameInputLabel LeftShoulderButtonLabel;
    public GameInputLabel RightShoulderButtonLabel;
    public GameInputLabel LeftThumbstickButtonLabel;
    public GameInputLabel RightThumbstickButtonLabel;
    public uint extraButtonCount;
    public uint extraAxisCount;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputRacingWheelInfo
{
    public GameInputLabel MenuButtonLabel;
    public GameInputLabel ViewButtonLabel;
    public GameInputLabel PreviousGearButtonLabel;
    public GameInputLabel NextGearButtonLabel;
    public GameInputLabel DpadUpLabel;
    public GameInputLabel DpadDownLabel;
    public GameInputLabel DpadLeftLabel;
    public GameInputLabel DpadRightLabel;
    public GameInputLabel AButtonLabel;
    public GameInputLabel BButtonLabel;
    public GameInputLabel XButtonLabel;
    public GameInputLabel YButtonLabel;
    public GameInputLabel LeftThumbstickButtonLabel;
    public GameInputLabel RightThumbstickButtonLabel;
    public bool HasClutch;
    public bool HasHandbrake;
    public bool HasPatternShifter;
    public bool HasPatternShifterGear;
    public bool MinPatternShifterGear;
    public bool MaxPatternShifterGear;
    public float MaxWheelAngle;
    public uint extraButtonCount;
    public uint extraAxisCount;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputForceFeedbackMotorInfo
{
    public GameInputFeedbackAxes SupportedAxes;
    public bool IsConstantEffectSupported;
    public bool IsRampEffectSupported;
    public bool IsSineWaveEffectSupported;
    public bool IsSquareWaveEffectSupported;
    public bool IsTriangleWaveEffectSupported;
    public bool IsSawtoothUpWaveEffectSupported;
    public bool IsSawtoothDownWaveEffectSupported;
    public bool IsSpringEffectSupported;
    public bool IsFrictionEffectSupported;
    public bool IsDamperEffectSupported;
    public bool IsInteriaEffectSupported;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputRawDeviceReportInfo
{
    public GameInputRawDeviceReportKind Kind;
    public uint Id;
    public uint Size;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputUsage
{
    public UInt16 page;
    public UInt16 id;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputDeviceInfo
{
    public UInt16 VendorId;
    public UInt16 ProductId;
    public UInt16 RevisionNumber;
    public GameInputUsage Usage;
    public GameInputVersion HardwareVersion;
    public GameInputVersion FirmwareVersion;
    public AppLocalDeviceId DeviceId;
    public AppLocalDeviceId DeviceRootId;
    public GameInputDeviceFamily DeviceFamily;
    public GameInputKind SupportedInput;
    public GameInputRumbleMotors SupportedRumbleMotors;
    public GameInputSystemButtons SupportedSystembuttons;
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

    public ReadOnlySpan<GameInputForceFeedbackMotorInfo> GetForceFeedbackMotorInfo()
    {
        if (ForceFeedbackMotorCount == 0) return ReadOnlySpan<GameInputForceFeedbackMotorInfo>.Empty;

        unsafe
        {
            return ForceFeedbackMotorInfo is null
                ? null
                : new ReadOnlySpan<GameInputForceFeedbackMotorInfo>(ForceFeedbackMotorInfo,
                    checked((int)ForceFeedbackMotorCount));
        }
    }

    public ReadOnlySpan<GameInputRawDeviceReportInfo> GetInputReportInfo()
    {
        if (InputReportCount == 0) return ReadOnlySpan<GameInputRawDeviceReportInfo>.Empty;

        unsafe
        {
            return InputReportInfo is null
                ? null
                : new ReadOnlySpan<GameInputRawDeviceReportInfo>(InputReportInfo,
                    checked((int)InputReportCount));
        }
    }

    public ReadOnlySpan<GameInputRawDeviceReportInfo> GetOutputReportInfo()
    {
        if (OutputReportCount == 0) return ReadOnlySpan<GameInputRawDeviceReportInfo>.Empty;

        unsafe
        {
            return OutputReportInfo is null
                ? null
                : new ReadOnlySpan<GameInputRawDeviceReportInfo>(OutputReportInfo,
                    checked((int)OutputReportCount));
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe partial struct GameInputHapticInfo
{
    private fixed char _audioEndpointId[Constants.GAMEINPUT_HAPTIC_MAX_AUDIO_ENDPOINT_ID_SIZE];
    public uint LocationCount;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.GAMEINPUT_HAPTIC_MAX_LOCATIONS)]
    private Guid[] _locations;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputForceFeedbackEnvelope
{
    public UInt64 AttackDuration;
    public UInt64 SustainDuration;
    public UInt64 ReleaseDuration;
    public float AttackGain;
    public float SustainGain;
    public float ReleaseGain;
    public uint PlayCount;
    public UInt64 RepeatDelay;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputForceFeedbackMagnitude
{
    public float LinearX;
    public float LinearY;
    public float LinearZ;
    public float AngularX;
    public float AngularY;
    public float AngularZ;
    public float Normal;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputForceFeedbackConditionParams
{
    public GameInputForceFeedbackMagnitude magnitude;
    public float PositiveCoefficient;
    public float NegativeCoefficient;
    public float MaxPositiveMagnitude;
    public float MaxNegativeMagnitude;
    public float DeadZone;
    public float Bias;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputForceFeedbackConstantParams
{
    public GameInputForceFeedbackEnvelope Envelope;
    public GameInputForceFeedbackMagnitude Magnitude;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputForceFeedbackPeriodicParams
{
    public GameInputForceFeedbackEnvelope Envelope;
    public GameInputForceFeedbackMagnitude Magnitude;
    public float Frequency;
    public float Phase;
    public float Bias;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GameInputForceFeedbackRampParams
{
    public GameInputForceFeedbackEnvelope Envelope;
    public GameInputForceFeedbackMagnitude StartMagnitude;
    public GameInputForceFeedbackMagnitude EndMagnitude;
}

[StructLayout(LayoutKind.Sequential)]
internal partial struct GameInputForceFeedbackParams
{
    public GameInputForceFeedbackEffectKind Kind;
    public GameInputForceFeedbackData Data;

    [StructLayout(LayoutKind.Explicit)]
    internal struct GameInputForceFeedbackData
    {
        [FieldOffset(0)] public GameInputForceFeedbackConstantParams Constant;
        [FieldOffset(0)] public GameInputForceFeedbackRampParams Ramp;
        [FieldOffset(0)] public GameInputForceFeedbackPeriodicParams SineWave;
        [FieldOffset(0)] public GameInputForceFeedbackPeriodicParams SquareWave;
        [FieldOffset(0)] public GameInputForceFeedbackPeriodicParams TriangleWave;
        [FieldOffset(0)] public GameInputForceFeedbackPeriodicParams SawtoothUpWave;
        [FieldOffset(0)] public GameInputForceFeedbackPeriodicParams SawtoothDownWave;
        [FieldOffset(0)] public GameInputForceFeedbackConditionParams Spring;
        [FieldOffset(0)] public GameInputForceFeedbackConditionParams Friction;
        [FieldOffset(0)] public GameInputForceFeedbackConditionParams Damper;
        [FieldOffset(0)] public GameInputForceFeedbackConditionParams Inertia;
    }
}

internal partial struct GameInputForceFeedbackParams
{
    // Constant
    public static GameInputForceFeedbackParams CreateConstant(GameInputForceFeedbackConstantParams payload)
    {
        return new GameInputForceFeedbackParams
        {
            Kind = GameInputForceFeedbackEffectKind.Constant,
            Data = new GameInputForceFeedbackData { Constant = payload }
        };
    }

    public GameInputForceFeedbackConstantParams GetConstant()
    {
        if (Kind != GameInputForceFeedbackEffectKind.Constant)
            throw new InvalidOperationException($"Kind {Kind} does not contain a Constant payload.");

        return Data.Constant;
    }

    // Ramp
    public static GameInputForceFeedbackParams CreateRamp(GameInputForceFeedbackRampParams payload)
    {
        return new GameInputForceFeedbackParams
        {
            Kind = GameInputForceFeedbackEffectKind.Ramp,
            Data = new GameInputForceFeedbackData { Ramp = payload }
        };
    }

    public GameInputForceFeedbackRampParams GetRamp()
    {
        if (Kind != GameInputForceFeedbackEffectKind.Ramp)
            throw new InvalidOperationException($"Kind {Kind} does not contain a Ramp payload.");
        return Data.Ramp;
    }

    // SineWave
    public static GameInputForceFeedbackParams CreateSineWave(GameInputForceFeedbackPeriodicParams payload)
    {
        return new GameInputForceFeedbackParams
        {
            Kind = GameInputForceFeedbackEffectKind.SineWave,
            Data = new GameInputForceFeedbackData { SineWave = payload }
        };
    }

    public GameInputForceFeedbackPeriodicParams GetSineWave()
    {
        if (Kind != GameInputForceFeedbackEffectKind.SineWave)
            throw new InvalidOperationException($"Kind {Kind} does not contain SineWave payload.");
        return Data.SineWave;
    }

    // SquareWave
    public static GameInputForceFeedbackParams CreateSquareWave(GameInputForceFeedbackPeriodicParams payload)
    {
        return new GameInputForceFeedbackParams
        {
            Kind = GameInputForceFeedbackEffectKind.SquareWave,
            Data = new GameInputForceFeedbackData { SquareWave = payload }
        };
    }

    public GameInputForceFeedbackPeriodicParams GetSquareWave()
    {
        if (Kind != GameInputForceFeedbackEffectKind.SquareWave)
            throw new InvalidOperationException($"Kind {Kind} does not contain SquareWave payload.");
        return Data.SquareWave;
    }

    // Triangle Wave
    public static GameInputForceFeedbackParams CreateTriangleWave(GameInputForceFeedbackPeriodicParams payload)
    {
        return new GameInputForceFeedbackParams
        {
            Kind = GameInputForceFeedbackEffectKind.TriangleWave,
            Data = new GameInputForceFeedbackData { TriangleWave = payload }
        };
    }

    public GameInputForceFeedbackPeriodicParams GetTriangleWave()
    {
        if (Kind != GameInputForceFeedbackEffectKind.TriangleWave)
            throw new InvalidOperationException($"Kind {Kind} does not contain TriangleWave payload.");
        return Data.TriangleWave;
    }

    // SawtoothUp Wave
    public static GameInputForceFeedbackParams CreateSawtoothUpWave(GameInputForceFeedbackPeriodicParams payload)
    {
        return new GameInputForceFeedbackParams
        {
            Kind = GameInputForceFeedbackEffectKind.SawtoothUpWave,
            Data = new GameInputForceFeedbackData { SawtoothUpWave = payload }
        };
    }

    public GameInputForceFeedbackPeriodicParams GetSawtoothUpWave()
    {
        if (Kind != GameInputForceFeedbackEffectKind.SawtoothUpWave)
            throw new InvalidOperationException($"Kind {Kind} does not contain SawtoothUpWave payload.");
        return Data.SawtoothUpWave;
    }

    // SawtoothDown Wave
    public static GameInputForceFeedbackParams CreateSawtoothDownWave(GameInputForceFeedbackPeriodicParams payload)
    {
        return new GameInputForceFeedbackParams
        {
            Kind = GameInputForceFeedbackEffectKind.SawtoothDownWave,
            Data = new GameInputForceFeedbackData { SawtoothDownWave = payload }
        };
    }

    public GameInputForceFeedbackPeriodicParams GetSawtoothDownWave()
    {
        if (Kind != GameInputForceFeedbackEffectKind.SawtoothDownWave)
            throw new InvalidOperationException($"Kind {Kind} does not contain SawtoothDownWave payload.");
        return Data.SawtoothDownWave;
    }

    // Spring
    public static GameInputForceFeedbackParams CreateSpring(GameInputForceFeedbackConditionParams payload)
    {
        return new GameInputForceFeedbackParams
        {
            Kind = GameInputForceFeedbackEffectKind.Spring,
            Data = new GameInputForceFeedbackData { Spring = payload }
        };
    }

    public GameInputForceFeedbackConditionParams GetSpring()
    {
        if (Kind != GameInputForceFeedbackEffectKind.Spring)
            throw new InvalidOperationException($"Kind {Kind} does not contain Spring payload.");
        return Data.Spring;
    }
}