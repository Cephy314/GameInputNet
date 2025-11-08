using System.Runtime.InteropServices;

namespace GameInputDotNet.Interop.Primitives;

/// <summary>
///     Represents a native <c>bool</c> value with explicit 1-byte storage.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct GameInputBoolean
{
    private byte _value;

    public GameInputBoolean(bool value)
    {
        _value = value ? (byte)1 : (byte)0;
    }

    public bool Value
    {
        readonly get => _value != 0;
        set => _value = value ? (byte)1 : (byte)0;
    }

    public static implicit operator bool(GameInputBoolean value) => value.Value;

    public static implicit operator GameInputBoolean(bool value) => new(value);
}
