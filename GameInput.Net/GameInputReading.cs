using System.Runtime.Versioning;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Handles;
using GameInputDotNet.Interop.Interfaces;
using GameInputDotNet.Interop.Structs;
using GameInputDotNet.States;

namespace GameInputDotNet;

/// <summary>
///     Managed wrapper over the natice <c>IGameInputReading</c> interface.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed unsafe class GameInputReading : IDisposable
{
    private GameInputReadingHandle? _handle;

    internal GameInputReading(GameInputReadingHandle handle)
    {
        ArgumentNullException.ThrowIfNull(handle);
        _handle = handle;
    }

    internal IGameInputReading NativeInterface =>
        _handle?.GetInterface() ?? throw new ObjectDisposedException(nameof(GameInputReading));

    public void Dispose()
    {
        _handle?.Dispose();
        _handle = null;
        GC.SuppressFinalize(this);
    }

    public GameInputKind GetInputKind()
    {
        return NativeInterface.GetInputKind();
    }

    public ulong GetTimestamp()
    {
        return NativeInterface.GetTimestamp();
    }

    public GameInputDevice GetDevice()
    {
        NativeInterface.GetDevice(out var device);

        if (device is null)
        {
            throw new GameInputException("IGameInputReading.GetDevice returned null.", unchecked((int)0x80004003));
        }

        var status = device.GetDeviceStatus();
        var timeStamp = NativeInterface.GetTimestamp();
        var handle = GameInputDeviceHandle.FromInterface(device);
        return new GameInputDevice(handle, timeStamp, status, status);
    }

    public ControllerState GetControllerState()
    {
        var axisCount = NativeInterface.GetControllerAxisCount();
        var axes = axisCount == 0 ? Array.Empty<float>() : new float[axisCount];
        if (axisCount > 0)
        {
            fixed (float* ptr = axes)
            {
                NativeInterface.GetControllerAxisState(axisCount, ptr);
            }
        }

        var buttonCount = NativeInterface.GetControllerButtonCount();
        var buttons = buttonCount == 0 ? Array.Empty<bool>() : new bool[buttonCount];
        if (buttonCount > 0)
        {
            fixed (bool* ptr = buttons)
            {
                NativeInterface.GetControllerButtonState(buttonCount, ptr);
            }
        }

        var switchCount = NativeInterface.GetControllerSwitchCount();
        var switches = switchCount == 0 ? Array.Empty<GameInputSwitchPosition>() : new GameInputSwitchPosition[switchCount];
        if (switchCount > 0)
        {
            fixed (GameInputSwitchPosition* ptr = switches)
            {
                NativeInterface.GetControllerSwitchState(switchCount, ptr);
            }
        }

        return new ControllerState(axes, buttons, switches);
    }

    public KeyboardState GetKeyboardState()
    {
        var keyCount = NativeInterface.GetKeyCount();
        var keys = keyCount == 0 ? Array.Empty<GameInputKeyState>() : new GameInputKeyState[keyCount];
        if (keyCount > 0)
        {
            fixed (GameInputKeyState* ptr = keys)
            {
                NativeInterface.GetKeyState(keyCount, ptr);
            }
        }

        return new KeyboardState(keys);
    }

    public MouseStateSnapshot? GetMouseState()
    {
        GameInputMouseState state;
        return NativeInterface.GetMouseState(&state) ? new MouseStateSnapshot(state) : null;
    }

    public SensorsStateSnapshot? GetSensorsState()
    {
        GameInputSensorsState state;
        return NativeInterface.GetSensorsState(&state) ? new SensorsStateSnapshot(state) : null;
    }

    public ArcadeStickStateSnapshot? GetArcadeStickState()
    {
        GameInputArcadeStickState state;
        return NativeInterface.GetArcadeStickState(&state) ? new ArcadeStickStateSnapshot(state) : null;
    }

    public FlightStickStateSnapshot? GetFlightStickState()
    {
        GameInputFlightStickState state;
        return NativeInterface.GetFlightStickState(&state) ? new FlightStickStateSnapshot(state) : null;
    }

    public GamepadStateSnapshot? GetGamepadState()
    {
        GameInputGamepadState state;
        return NativeInterface.GetGamepadState(&state) ? new GamepadStateSnapshot(state) : null;
    }

    public RacingWheelStateSnapshot? GetRacingWheelState()
    {
        GameInputRacingWheelState state;
        return NativeInterface.GetRacingWheelState(&state) ? new RacingWheelStateSnapshot(state) : null;
    }

    public GameInputRawDeviceReport? GetRawReport()
    {
        return NativeInterface.GetRawReport(out var nativeReport) && nativeReport is not null
            ? new GameInputRawDeviceReport(GameInputRawDeviceReportHandle.FromInterface(nativeReport))
            : null;
    }
}
