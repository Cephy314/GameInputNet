using System.Runtime.InteropServices;
using GameInputDotNet.Interop.Primitives;

namespace GameInputDotNet.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct GameInputKeyState
{
    public uint ScanCode;
    public uint CodePoint;
    public byte VirtualKey;

    public GameInputBoolean IsDeadKey;
}
