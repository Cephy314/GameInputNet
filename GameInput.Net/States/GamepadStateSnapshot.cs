using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Structs;

namespace GameInputDotNet.States;

public sealed class GamepadStateSnapshot
{
    internal GamepadStateSnapshot(GameInputGamepadState state)
    {
        Buttons = state.Buttons;
        LeftTrigger = state.LeftTrigger;
        RightTrigger = state.RightTrigger;
        LeftThumbstickX = state.LeftThumbstickX;
        LeftThumbstickY = state.LeftThumbstickY;
        RightThumbstickX = state.RightThumbstickX;
        RightThumbstickY = state.RightThumbstickY;
    }

    public GameInputGamepadButtons Buttons { get; }
    public float LeftTrigger { get; }
    public float RightTrigger { get; }
    public float LeftThumbstickX { get; }
    public float LeftThumbstickY { get; }
    public float RightThumbstickX { get; }
    public float RightThumbstickY { get; }
}
