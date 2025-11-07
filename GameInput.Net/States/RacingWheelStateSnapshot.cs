using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Structs;

namespace GameInputDotNet.States;

public sealed class RacingWheelStateSnapshot
{
    internal RacingWheelStateSnapshot(GameInputRacingWheelState state)
    {
        Buttons = state.Buttons;
        PatternShifterGear = state.PatternShifterGear;
        Wheel = state.Wheel;
        Throttle = state.Throttle;
        Brake = state.Brake;
        Clutch = state.Clutch;
        Handbrake = state.Handbrake;
    }

    public GameInputRacingWheelButtons Buttons { get; }
    public int PatternShifterGear { get; }
    public float Wheel { get; }
    public float Throttle { get; }
    public float Brake { get; }
    public float Clutch { get; }
    public float Handbrake { get; }
}
