using System.Runtime.InteropServices;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Primitives;

namespace GameInputDotNet.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct GameInputRacingWheelInfo
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

    public GameInputBoolean HasClutch;

    public GameInputBoolean HasHandbrake;

    public GameInputBoolean HasPatternShifter;

    public int MinPatternShifterGear;

    public int MaxPatternShifterGear;

    public float MaxWheelAngle;
    public uint extraButtonCount;
    public uint extraAxisCount;
}
