using System.Runtime.InteropServices;
using GameInputDotNet.Interop.Primitives;

namespace GameInputDotNet.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct GameInputKeyState
{
    public uint ScanCode;
    public uint CodePoint;
    public byte VirtualKey;
    internal GameInputBoolean _isDeadKey;

    public bool IsDeadKey => _isDeadKey.Value;
}