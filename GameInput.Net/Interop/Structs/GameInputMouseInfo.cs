using System.Runtime.InteropServices;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Primitives;

namespace GameInputDotNet.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct GameInputMouseInfo
{
    public GameInputMouseButtons SupportedButtons;
    public uint SampleRate;

    public GameInputBoolean HasWheelX;

    public GameInputBoolean HasWheelY;
}
