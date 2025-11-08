using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using GameInputDotNet.Interop.Interfaces;
using Microsoft.Win32.SafeHandles;

namespace GameInputDotNet.Interop.Handles;

/// <summary>
///     Safe handle managing the lifetime of an <see cref="IGameInputForceFeedbackEffect" /> COM pointer.
/// </summary>
[SupportedOSPlatform("windows")]
internal sealed class GameInputForceFeedbackEffectHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private IGameInputForceFeedbackEffect? _effect;

    private GameInputForceFeedbackEffectHandle()
        : base(true)
    {
    }

    public static GameInputForceFeedbackEffectHandle FromInterface(IGameInputForceFeedbackEffect effect)
    {
        ArgumentNullException.ThrowIfNull(effect);

        var handle = new GameInputForceFeedbackEffectHandle
        {
            handle = Marshal.GetIUnknownForObject(effect)
        };

        // Marshal.GetIUnknownForObject increments the COM ref count; hold onto the RCW so we can release the
        // same reference during Dispose.
        handle._effect = effect;
        return handle;
    }

    public IGameInputForceFeedbackEffect GetInterface()
    {
        if (handle == IntPtr.Zero)
        {
            throw new ObjectDisposedException(nameof(GameInputForceFeedbackEffectHandle), "GameInputForceFeedbackEffectHandle object can not be disposed.");
        }

        return _effect ??= (IGameInputForceFeedbackEffect)Marshal.GetObjectForIUnknown(handle);
    }

    protected override bool ReleaseHandle()
    {
        Marshal.Release(handle);
        _effect = null;
        return true;
    }
}
