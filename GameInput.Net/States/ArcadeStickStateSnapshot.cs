using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Structs;

namespace GameInputDotNet.States;

public sealed class ArcadeStickStateSnapshot
{
    internal ArcadeStickStateSnapshot(GameInputArcadeStickState state)
    {
        Buttons = state.Buttons;
    }

    public GameInputArcadeStickButtons Buttons { get; }
}
