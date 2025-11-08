using System.Runtime.InteropServices;
using GameInputDotNet.Interop.Primitives;

namespace GameInputDotNet.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct GameInputKeyState
{
    public uint ScanCode;
    public uint CodePoint;
    internal GameInputBoolean _virtualKey;
    internal GameInputBoolean _isDeadKey;

    public bool IsDeadKey => _isDeadKey.Value;
    public bool IsVirtualKey => _virtualKey.Value;
}