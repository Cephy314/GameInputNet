using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GameInputDotNet.Interop.Enums;

namespace GameInputDotNet.States;

public sealed class ControllerState
{
    internal ControllerState(float[] axes, bool[] buttons, GameInputSwitchPosition[] switches)
    {
        Axes = Array.AsReadOnly(axes);
        Buttons = Array.AsReadOnly(buttons);
        Switches = Array.AsReadOnly(switches);
    }

    public IReadOnlyList<float> Axes { get; }

    public IReadOnlyList<bool> Buttons { get; }

    public IReadOnlyList<GameInputSwitchPosition> Switches { get; }
}
