using System.Runtime.InteropServices;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Primitives;

namespace GameInputDotNet.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct GameInputAxisMapping
{
    public GameInputElementKind ControllerElementKind;
    public uint ControllerIndex;

    public GameInputBoolean IsInverted;

    public GameInputBoolean FromTwoButtons;

    public uint ButtonMinIndexValue;
    public GameInputSwitchPosition ReferenceDirection;
}
