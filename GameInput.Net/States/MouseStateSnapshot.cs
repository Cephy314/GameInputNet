using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Structs;

namespace GameInputDotNet.States;

public sealed class MouseStateSnapshot
{
    internal MouseStateSnapshot(GameInputMouseState state)
    {
        Buttons = state.Buttons;
        Positions = state.Positions;
        PositionX = state.PositionX;
        PositionY = state.PositionY;
        AbsolutePositionX = state.AbsolutePositionX;
        AbsolutePositionY = state.AbsolutePositionY;
        WheelX = state.WheelX;
        WheelY = state.WheelY;
    }

    public GameInputMouseButtons Buttons { get; }
    public GameInputMousePositions Positions { get; }
    public long PositionX { get; }
    public long PositionY { get; }
    public long AbsolutePositionX { get; }
    public long AbsolutePositionY { get; }
    public long WheelX { get; }
    public long WheelY { get; }
}
