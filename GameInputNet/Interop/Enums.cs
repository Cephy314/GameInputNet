namespace GameInputNet.Interop;

[Flags]
public enum GameInputKind : uint
{
    Unknown = 0x00000000,
    RawDeviceReport = 0x00000001,
    ControllerAxis = 0x00000002,
    ControllerButton = 0x00000004,
    ControllerSwitch = 0x00000008,
    Controller = 0x0000000E,
    Keyboard = 0x00000010,
    Mouse = 0x00000020,
    Sensors = 0x00000040,
    ArcadeStick = 0x00010000,
    FlightStick = 0x00020000,
    Gamepad = 0x00040000,
    RacingWheel = 0x00080000
}

public enum GameInputEnumerationKind : uint
{
    None = 0,
    Async = 1,
    Blocking = 2
}

[Flags]
public enum GameInputFocusPolicy : uint
{
    Default = 0x00000000,
    ExclusiveForegroundInput = 0x00000002,
    ExclusiveForegroundGuideButton = 0x00000008,
    ExclusiveForegroundShareButton = 0x00000020,
    EnableBackgroundInput = 0x00000040,
    EnableBackgroundGuideButton = 0x00000080,
    EnableBackgroundShareButton = 0x00000100
}

[Flags]
public enum GameInputDeviceStatus : uint
{
    None = 0x00000000,
    Connected = 0x00000001,
    HapticInfoReady = 0x00200000,
    Any = 0xFFFFFFFF
}

[Flags]
public enum GameInputSystemButtons : uint
{
    None = 0x00000000,
    Guide = 0x00000001,
    Share = 0x00000002,
    Menu = 0x00000004,

    View = 0x00000008
    // Fill out the rest as you translate additional bit fields from GameInput.h.
}

[Flags]
public enum GameInputMouseButtons : uint
{
    None = 0x00000000,
    Left = 0x00000001,
    Right = 0x00000002,
    Middle = 0x00000004,
    Button4 = 0x00000008,
    Button5 = 0x00000010,
    WheelTiltLeft = 0x00000020,
    WheelTiltRight = 0x00000040
}

internal enum GameInputSwitchKind
{
    Unknown = -1,
    TwoWaySwitch = 0,
    FourWaySwitch = 1,
    EightWaySwitch = 2
}

public enum GameInputSwitchPosition
{
    Center = 0,
    Up = 1,
    UpRight = 2,
    Right = 3,
    DownRight = 4,
    Down = 5,
    DownLeft = 6,
    Left = 7,
    UpLeft = 8
}

internal enum GameInputKeyboardKind
{
    Unknown = -1,
    Ansi = 0,
    Iso = 1,
    Ks = 2,
    Abnt = 3,
    Jis = 4
}

[Flags]
public enum GameInputMousePositions : uint
{
    NoPosition = 0x00000000,
    AbsolutePosition = 0x00000001,
    RelativePosition = 0x00000002
}

[Flags]
public enum GameInputSensorsKind : uint
{
    None = 0x00000000,
    Accelerometer = 0x00000001,
    Gyrometer = 0x00000002,
    Compass = 0x00000004,
    Orientation = 0x00000008
}

public enum GameInputSensorAccuracy : uint
{
    Unknown = 0x00000000,
    Unreliable = 0x00000001,
    Approximate = 0x00000002,
    High = 0x00000003
}

[Flags]
public enum GameInputArcadeStickButtons : uint
{
    None = 0x00000000,
    Menu = 0x00000001,
    View = 0x00000002,
    Up = 0x00000004,
    Down = 0x00000008,
    Left = 0x00000010,
    Right = 0x00000020,
    Action1 = 0x00000040,
    Action2 = 0x00000080,
    Action3 = 0x00000100,
    Action4 = 0x00000200,
    Action5 = 0x00000400,
    Action6 = 0x00000800,
    Special1 = 0x00001000,
    Special2 = 0x00002000
}

[Flags]
public enum GameInputFlightStickButtons : uint
{
    None = 0x00000000,
    Menu = 0x00000001,
    View = 0x00000002,
    FirePrimary = 0x00000004,
    FireSecondary = 0x00000008,
    HatSwitchUp = 0x00000010,
    HatSwitchDown = 0x00000020,
    HatSwitchLeft = 0x00000040,
    HatSwitchRight = 0x00000080,
    A = 0x00000100,
    B = 0x00000200,
    X = 0x00000400,
    Y = 0x00000800,
    LeftShoulder = 0x00001000,
    RightShoulder = 0x00002000
}

[Flags]
internal enum GameInputGamepadButtons : uint
{
    None = 0x00000000,
    Menu = 0x00000001,
    View = 0x00000002,
    A = 0x00000004,
    B = 0x00000008,
    C = 0x00004000,
    X = 0x00000010,
    Y = 0x00000020,
    Z = 0x00008000,
    DPadUp = 0x00000040,
    DPadDown = 0x00000080,
    DPadLeft = 0x00000100,
    DPadRight = 0x00000200,
    LeftShoulder = 0x00000400,
    RightShoulder = 0x00000800,
    LeftTriggerButton = 0x00010000,
    RightTriggerButton = 0x00020000,
    LeftThumbstick = 0x00001000,
    LeftThumbstickUp = 0x00040000,
    LeftThumbstickDown = 0x00080000,
    LeftThumbstickLeft = 0x00100000,
    LeftThumbstickRight = 0x00200000,
    RightThumbstick = 0x00002000,
    RightThumbstickUp = 0x00400000,
    RightThumbstickDown = 0x00800000,
    RightThumbstickLeft = 0x01000000,
    RightThumbstickRight = 0x02000000,
    PaddleLeft1 = 0x04000000,
    PaddleLeft2 = 0x08000000,
    PaddleRight1 = 0x10000000,
    PaddleRight2 = 0x20000000
}

internal enum GameInputRawDeviceReportKind
{
    InputReport = 0,
    OutputReport = 1
}

[Flags]
internal enum GameInputRacingWheelButtons : uint
{
    None = 0x00000000,
    Menu = 0x00000001,
    View = 0x00000002,
    PreviousGear = 0x00000004,
    NextGear = 0x00000008,
    A = 0x00000100,
    B = 0x00000200,
    X = 0x00000400,
    Y = 0x00000800,
    DpadUp = 0x00000010,
    DpadDown = 0x00000020,
    DpadLeft = 0x00000040,
    DpadRight = 0x00000080,
    LeftThumbstick = 0x00001000,
    RightThumbstick = 0x00002000
}

[Flags]
internal enum GameInputFlightStickAxes : uint
{
    AxesNone = 0x00000000,
    Roll = 0x00000010,
    Pitch = 0x00000020,
    Yaw = 0x00000040,
    Throttle = 0x00000080
}

[Flags]
internal enum GameInputGamepadAxes : uint
{
    AxesNone = 0x00000000,
    LeftTrigger = 0x00000001,
    RightTrigger = 0x00000002,
    LeftThumbstickX = 0x00000004,
    LeftThumbstickY = 0x00000008,
    RightThumbstickX = 0x00000010,
    RightThumbstickY = 0x00000020
}

[Flags]
internal enum GameInputRacingWheelAxes : uint
{
    AxesNone = 0x00000000,
    Steering = 0x00000100,
    Throttle = 0x00000200,
    Brake = 0x00000400,
    Clutch = 0x00000800,
    Handbrake = 0x00001000,
    PatternShifter = 0x00002000
}

public enum GameInputDeviceFamily
{
    Virtual = -1,
    Unknown = 0,
    XboxOne = 1,
    Xbox360 = 2,
    Hid = 3,
    I8042 = 4,
    Aggregate = 5
}

public enum GameInputLabel
{
    Unknown = -1,
    None = 0,
    XboxGuide = 1,
    XboxBack = 2,
    XboxStart = 3,
    XboxMenu = 4,
    XboxView = 5,
    XboxA = 7,
    XboxB = 8,
    XboxX = 9,
    XboxY = 10,
    XboxDPadUp = 11,
    XboxDPadDown = 12,
    XboxDPadLeft = 13,
    XboxDPadRight = 14,
    XboxLeftShoulder = 15,
    XboxLeftTrigger = 16,
    XboxLeftStickButton = 17,
    XboxRightShoulder = 18,
    XboxRightTrigger = 19,
    XboxRightStickButton = 20,
    XboxPaddle1 = 21,
    XboxPaddle2 = 22,
    XboxPaddle3 = 23,
    XboxPaddle4 = 24,
    LetterA = 25,
    LetterB = 26,
    LetterC = 27,
    LetterD = 28,
    LetterE = 29,
    LetterF = 30,
    LetterG = 31,
    LetterH = 32,
    LetterI = 33,
    LetterJ = 34,
    LetterK = 35,
    LetterL = 36,
    LetterM = 37,
    LetterN = 38,
    LetterO = 39,
    LetterP = 40,
    LetterQ = 41,
    LetterR = 42,
    LetterS = 43,
    LetterT = 44,
    LetterU = 45,
    LetterV = 46,
    LetterW = 47,
    LetterX = 48,
    LetterY = 49,
    LetterZ = 50,
    Number0 = 51,
    Number1 = 52,
    Number2 = 53,
    Number3 = 54,
    Number4 = 55,
    Number5 = 56,
    Number6 = 57,
    Number7 = 58,
    Number8 = 59,
    Number9 = 60,
    ArrowUp = 61,
    ArrowUpRight = 62,
    ArrowRight = 63,
    ArrowDownRight = 64,
    ArrowDown = 65,
    ArrowDownLeft = 66,
    ArrowLeft = 67,
    ArrowUpLeft = 68,
    ArrowUpDown = 69,
    ArrowLeftRight = 70,
    ArrowUpDownLeftRight = 71,
    ArrowClockwise = 72,
    ArrowCounterClockwise = 73,
    ArrowReturn = 74,
    IconBranding = 75,
    IconHome = 76,
    IconMenu = 77,
    IconCross = 78,
    IconCircle = 79,
    IconSquare = 80,
    IconTriangle = 81,
    IconStar = 82,
    IconDPadUp = 83,
    IconDPadDown = 84,
    IconDPadLeft = 85,
    IconDPadRight = 86,
    IconDialClockwise = 87,
    IconDialCounterClockwise = 88,
    IconSliderLeftRight = 89,
    IconSliderUpDown = 90,
    IconWheelUpDown = 91,
    IconPlus = 92,
    IconMinus = 93,
    IconSuspension = 94,
    Home = 95,
    Guide = 96,
    Mode = 97,
    Select = 98,
    Menu = 99,
    View = 100,
    Back = 101,
    Start = 102,
    Options = 103,
    Share = 104,
    Up = 105,
    Down = 106,
    Left = 107,
    Right = 108,
    LB = 109,
    LT = 110,
    LSB = 111,
    L1 = 112,
    L2 = 113,
    L3 = 114,
    RB = 115,
    RT = 116,
    RSB = 117,
    R1 = 118,
    R2 = 119,
    R3 = 120,
    PaddleLeft1 = 121,
    PaddleLeft2 = 122,
    PaddleRight1 = 123,
    PaddleRight2 = 124
}

[Flags]
public enum GameInputFeedbackAxes : uint
{
    GameInputFeedbackAxisNone = 0x00000000,
    GameInputFeedbackAxisLinearX = 0x00000001,
    GameInputFeedbackAxisLinearY = 0x00000002,
    GameInputFeedbackAxisLinearZ = 0x00000004,
    GameInputFeedbackAxisAngularX = 0x00000008,
    GameInputFeedbackAxisAngularY = 0x00000010,
    GameInputFeedbackAxisAngularZ = 0x00000020,
    GameInputFeedbackAxisNormal = 0x00000040
}

public enum GameInputFeedbackEffectState
{
    Stopped = 0,
    Running = 1,
    Paused = 2
}

public enum GameInputForceFeedbackEffectKind
{
    Constant = 0,
    Ramp = 1,
    SineWave = 2,
    SquareWave = 3,
    TriangleWave = 4,
    SawtoothUpWave = 5,
    SawtoothDownWave = 6,
    Spring = 7,
    Friction = 8,
    Damper = 9,
    Inertia = 10
}

[Flags]
public enum GameInputRumbleMotors
{
    None = 0x00000000,
    LowFrequency = 0x00000001,
    HighFrequency = 0x00000002,
    LeftTrigger = 0x00000004,
    RightTrigger = 0x00000008
}
