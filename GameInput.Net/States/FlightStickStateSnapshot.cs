using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Structs;

namespace GameInputDotNet.States;

public sealed class FlightStickStateSnapshot
{
    internal FlightStickStateSnapshot(GameInputFlightStickState state)
    {
        Buttons = state.Buttons;
        HatSwitch = state.HatSwitch;
        Roll = state.Roll;
        Pitch = state.Pitch;
        Yaw = state.Yaw;
        Throttle = state.Throttle;
    }

    public GameInputFlightStickButtons Buttons { get; }
    public GameInputSwitchPosition HatSwitch { get; }
    public float Roll { get; }
    public float Pitch { get; }
    public float Yaw { get; }
    public float Throttle { get; }
}
