using System.Runtime.Versioning;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Handles;
using GameInputDotNet.Interop.Interfaces;
using GameInputDotNet.Interop.Structs;

namespace GameInputDotNet;

/// <summary>
///     Managed wrapper over the native <c>IGameInputMapper</c> interface.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class GameInputMapper : IDisposable
{
    private GameInputMapperHandle? _handle;

    internal GameInputMapper(GameInputMapperHandle handle)
    {
        ArgumentNullException.ThrowIfNull(handle);
        _handle = handle;
    }

    internal IGameInputMapper NativeInterface =>
        _handle?.GetInterface() ?? throw new ObjectDisposedException(nameof(GameInputMapper));

    public void Dispose()
    {
        _handle?.Dispose();
        _handle = null;
        GC.SuppressFinalize(this);
    }

    public bool TryGetArcadeStickButtonMapping(GameInputArcadeStickButtons button, out GameInputButtonMapping mapping)
    {
        unsafe
        {
            fixed (GameInputButtonMapping* mappingPtr = &mapping)
            {
                return NativeInterface.GetArcadeStickButtonMappingInfo(button, mappingPtr);
            }
        }
    }

    public bool TryGetFlightStickAxisMapping(GameInputFlightStickAxes axis, out GameInputAxisMapping mapping)
    {
        unsafe
        {
            fixed (GameInputAxisMapping* mappingPtr = &mapping)
            {
                return NativeInterface.GetFlightStickAxisMappingInfo(axis, mappingPtr);
            }
        }
    }

    public bool TryGetFlightStickButtonMapping(GameInputFlightStickButtons button, out GameInputButtonMapping mapping)
    {
        unsafe
        {
            fixed (GameInputButtonMapping* mappingPtr = &mapping)
            {
                return NativeInterface.GetFlightStickButtonMappingInfo(button, mappingPtr);
            }
        }
    }

    public bool TryGetGamepadAxisMapping(GameInputGamepadAxes axis, out GameInputAxisMapping mapping)
    {
        unsafe
        {
            fixed (GameInputAxisMapping* mappingPtr = &mapping)
            {
                return NativeInterface.GetGamepadAxisMappingInfo(axis, mappingPtr);
            }
        }
    }

    public bool TryGetGamepadButtonMapping(GameInputGamepadButtons button, out GameInputButtonMapping mapping)
    {
        unsafe
        {
            fixed (GameInputButtonMapping* mappingPtr = &mapping)
            {
                return NativeInterface.GetGamepadButtonMappingInfo(button, mappingPtr);
            }
        }
    }

    public bool TryGetRacingWheelAxisMapping(GameInputRacingWheelAxes axis, out GameInputAxisMapping mapping)
    {
        unsafe
        {
            fixed (GameInputAxisMapping* mappingPtr = &mapping)
            {
                return NativeInterface.GetRacingWheelAxisMappingInfo(axis, mappingPtr);
            }
        }
    }

    public bool TryGetRacingWheelButtonMapping(GameInputRacingWheelButtons button, out GameInputButtonMapping mapping)
    {
        unsafe
        {
            fixed (GameInputButtonMapping* mappingPtr = &mapping)
            {
                return NativeInterface.GetRacingWheelButtonMappingInfo(button, mappingPtr);
            }
        }
    }
}
