using GameInputNet.Interop;
using static GameInputNet.Interop.GameInputNative;

namespace GameInputNet;

/// <summary>
/// Managed wrapper over the native <c>IGameInput</c> interface.
/// </summary>
public sealed class GameInput : IDisposable
{
    private GameInputHandle? _handle;

    internal GameInput(GameInputHandle handle)
    {
        ArgumentNullException.ThrowIfNull(handle);
        _handle = handle;
    }

    internal IGameInput NativeInterface =>
        _handle?.GetInterface() ?? throw new ObjectDisposedException(nameof(GameInput));

    /// <summary>
    /// Gets the current timestamp from the GameInput runtime.
    /// </summary>
    public ulong GetCurrentTimestamp() => NativeInterface.GetCurrentTimestamp();

    /// <summary>
    /// Applies the provided focus policy to the GameInput runtime.
    /// </summary>
    public void SetFocusPolicy(GameInputFocusPolicy policy) => NativeInterface.SetFocusPolicy(policy);

    /// <inheritdoc/>
    public void Dispose()
    {
        _handle?.Dispose();
        _handle = null;
        GC.SuppressFinalize(this);
    }
}
