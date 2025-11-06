using GameInputNet.Interop;
using static GameInputNet.Interop.GameInputNative;

namespace GameInputNet;

/// <summary>
/// Provides factory helpers for creating a managed <see cref="GameInput"/> wrapper around the native GameInput COM object.
/// </summary>
public static class GameInputFactory
{
    /// <summary>
    /// Creates a new <see cref="GameInput"/> instance backed by the native <c>IGameInput</c> interface.
    /// </summary>
    /// <exception cref="GameInputException">Thrown when the native <c>GameInputCreate</c> call fails.</exception>
    public static GameInput Create()
    {
        var hr = GameInputCreate(out var gameInput);
        GameInputException.ThrowIfFailed(hr, "GameInputCreate failed.");

        if (gameInput is null)
        {
            throw new GameInputException("GameInputCreate returned a null interface.", hr);
        }

        var handle = GameInputHandle.FromInterface(gameInput);
        return new GameInput(handle);
    }
}
