using System.Runtime.Versioning;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Handles;
using GameInputDotNet.Interop.Interfaces;
using GameInputDotNet.Interop.Structs;

namespace GameInputDotNet;

[SupportedOSPlatform("windows")]
public sealed class GameInputForceFeedbackEffect : IDisposable
{
    private GameInputForceFeedbackEffectHandle? _handle;

    internal GameInputForceFeedbackEffect(GameInputForceFeedbackEffectHandle handle)
    {
        ArgumentNullException.ThrowIfNull(handle);
        _handle = handle;
    }

    internal IGameInputForceFeedbackEffect NativeInterface =>
        _handle?.GetInterface() ?? throw new ObjectDisposedException(nameof(GameInputForceFeedbackEffect));

    public void Dispose()
    {
        _handle?.Dispose();
        _handle = null;
        GC.SuppressFinalize(this);
    }

    public GameInputDevice GetDevice()
    {
        NativeInterface.GetDevice(out var device);
        if (device is null)
        {
            throw new GameInputException("IGameInputForceFeedbackEffect.GetDevice returned null.", unchecked((int)0x80004003));
        }

        var status = device.GetDeviceStatus();
        var handle = GameInputDeviceHandle.FromInterface(device);
        return new GameInputDevice(handle, 0, status, status);
    }

    public uint GetMotorIndex()
    {
        return NativeInterface.GetMotorIndex();
    }

    public float GetGain()
    {
        return NativeInterface.GetGain();
    }

    public void SetGain(float gain)
    {
        NativeInterface.SetGain(gain);
    }

    public GameInputForceFeedbackParams GetParameters()
    {
        unsafe
        {
            GameInputForceFeedbackParams parameters = default;
            NativeInterface.GetParams(&parameters);
            return parameters;
        }
    }

    public void SetParameters(GameInputForceFeedbackParams parameters)
    {
        unsafe
        {
            var result = NativeInterface.SetParams(&parameters);
            if (!result)
            {
                throw new GameInputException("IGameInputForceFeedbackEffect.SetParams failed.", unchecked((int)0x80004005));
            }
        }
    }

    public GameInputFeedbackEffectState GetState()
    {
        return NativeInterface.GetState();
    }

    public void SetState(GameInputFeedbackEffectState state)
    {
        NativeInterface.SetState(state);
    }
}
