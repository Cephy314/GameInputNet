using System;
using System.Collections.Generic;
using GameInputDotNet.Interop.Structs;

namespace GameInputDotNet.States;

public sealed class KeyboardState
{
    internal KeyboardState(GameInputKeyState[] keys)
    {
        Keys = Array.AsReadOnly(keys);
    }

    public IReadOnlyList<GameInputKeyState> Keys { get; }
}
