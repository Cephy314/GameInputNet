using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using GameInputDotNet.Interop;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Handles;
using GameInputDotNet.Interop.Interfaces;
using GameInputDotNet.Interop.Structs;

namespace GameInputDotNet;

/// <summary>
///     Managed wrapper over a native <c>IGameInputDevice</c> interface.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class GameInputDevice : IDisposable
{
    private GameInputDeviceHandle? _handle;

    internal GameInputDevice(GameInputDeviceHandle handle, ulong timestamp, GameInputDeviceStatus currentStatus,
        GameInputDeviceStatus previousStatus)
    {
        ArgumentNullException.ThrowIfNull(handle);

        _handle = handle;
        LastSeenTimestamp = timestamp;
        CurrentStatus = currentStatus;
        PreviousStatus = previousStatus;
    }

    /// <summary>
    ///     Timestamp provided by the GameInput runtime when this device was observed.
    /// </summary>
    public ulong LastSeenTimestamp { get; }

    /// <summary>
    ///     Device status reported during enumeration.
    /// </summary>
    public GameInputDeviceStatus CurrentStatus { get; }

    /// <summary>
    ///     Previous status reported by the runtime prior to this enumeration result.
    /// </summary>
    public GameInputDeviceStatus PreviousStatus { get; }

    internal IGameInputDevice NativeInterface =>
        _handle?.GetInterface() ?? throw new ObjectDisposedException(nameof(GameInputDevice));

    /// <summary>
    ///     Retrieves the current device information block from the GameInput runtime.
    /// </summary>
    public unsafe GameInputDeviceInfo GetDeviceInfo()
    {
        var hr = NativeInterface.GetDeviceInfo(out GameInputDeviceInfo* info);
        GameInputException.ThrowIfFailed(hr, "IGameInputDevice.GetDeviceInfo failed.");

        if (info is null)
        {
            throw new GameInputException("IGameInputDevice.GetDeviceInfo returned a null pointer.", hr);
        }

        return *info;
    }

    /// <summary>
    ///     Retrieves the latest device status value from the GameInput runtime.
    /// </summary>
    public GameInputDeviceStatus GetCurrentStatus()
    {
        return NativeInterface.GetDeviceStatus();
    }

    /// <summary>
    ///     Retrieves the haptic configuration for this device.
    /// </summary>
    /// <exception cref="GameInputHapticInfoNotFoundException">
    ///     Thrown if the device does not expose haptics information.
    /// </exception>
    public unsafe GameInputHapticInfo GetHapticInfo()
    {
        GameInputHapticInfo info = default;
        var hr = NativeInterface.GetHapticInfo(&info);
        GameInputErrorMapper.ThrowIfFailed(hr, "IGameInputDevice.GetHapticInfo failed.");
        GC.KeepAlive(this);
        return info;
    }

    /// <summary>
    ///     Creates a new force feedback effect for the specified motor.
    /// </summary>
    /// <param name="motorIndex">Index of the motor to control.</param>
    /// <param name="parameters">Initial parameters for the effect.</param>
    /// <returns>A managed wrapper for the created force feedback effect.</returns>
    public GameInputForceFeedbackEffect CreateForceFeedbackEffect(uint motorIndex,
        GameInputForceFeedbackParams parameters)
    {
        unsafe
        {
            var localParams = parameters;
            var hr = NativeInterface.CreateForceFeedbackEffect(motorIndex, &localParams, out var effect);
            GameInputErrorMapper.ThrowIfFailed(hr, "IGameInputDevice.CreateForceFeedbackEffect failed.");

            if (effect is null)
            {
                throw new GameInputException("IGameInputDevice.CreateForceFeedbackEffect returned null.",
                    unchecked((int)0x80004003));
            }

            var handle = GameInputForceFeedbackEffectHandle.FromInterface(effect);
            GC.KeepAlive(this);
            return new GameInputForceFeedbackEffect(handle);
        }
    }

    /// <summary>
    ///     Determines whether the specified force feedback motor is currently powered on.
    /// </summary>
    public bool IsForceFeedbackMotorPoweredOn(uint motorIndex)
    {
        var result = NativeInterface.IsForceFeedbackMotorPoweredOn(motorIndex);
        GC.KeepAlive(this);
        return result;
    }

    /// <summary>
    ///     Sets the master gain applied to the specified force feedback motor.
    /// </summary>
    public void SetForceFeedbackMotorGain(uint motorIndex, float masterGain)
    {
        NativeInterface.SetForceFeedbackMotorGain(motorIndex, masterGain);
        GC.KeepAlive(this);
    }

    /// <summary>
    ///     Applies a rumble state to the device.
    /// </summary>
    public void SetRumbleState(GameInputRumbleParams parameters)
    {
        unsafe
        {
            var localParams = parameters;
            NativeInterface.SetRumbleState(&localParams);
        }

        GC.KeepAlive(this);
    }

    /// <summary>
    ///     Creates a raw device report for the specified identifier and kind.
    /// </summary>
    public GameInputRawDeviceReport CreateRawDeviceReport(uint reportId, GameInputRawDeviceReportKind reportKind)
    {
        var hr = NativeInterface.CreateRawDeviceReport(reportId, reportKind, out var report);
        GameInputErrorMapper.ThrowIfFailed(hr, "IGameInputDevice.CreateRawDeviceReport failed.");

        if (report is null)
        {
            throw new GameInputException("IGameInputDevice.CreateRawDeviceReport returned null.",
                unchecked((int)0x80004003));
        }

        var handle = GameInputRawDeviceReportHandle.FromInterface(report);
        GC.KeepAlive(this);
        return new GameInputRawDeviceReport(handle);
    }

    /// <summary>
    ///     Sends the provided raw device report to the device.
    /// </summary>
    public void SendRawDeviceOutput(GameInputRawDeviceReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        var hr = NativeInterface.SendRawDeviceOutput(report.NativeInterface);
        GameInputErrorMapper.ThrowIfFailed(hr, "IGameInputDevice.SendRawDeviceOutput failed.");

        GC.KeepAlive(this);
        GC.KeepAlive(report);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _handle?.Dispose();
        _handle = null;
        GC.SuppressFinalize(this);
    }
}
