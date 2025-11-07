using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using GameInputDotNet.Interop;
using GameInputDotNet.Interop.Delegates;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Handles;
using GameInputDotNet.Interop.Interfaces;

namespace GameInputDotNet;

/// <summary>
///     Managed wrapper over the native <c>IGameInput</c> interface.
/// </summary>
[SupportedOSPlatform("windows")]
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

    /// <inheritdoc />
    public void Dispose()
    {
        _handle?.Dispose();
        _handle = null;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Gets the current timestamp from the GameInput runtime.
    /// </summary>
    public ulong GetCurrentTimestamp()
    {
        return NativeInterface.GetCurrentTimestamp();
    }

    /// <summary>
    ///     Applies the provided focus policy to the GameInput runtime.
    /// </summary>
    public void SetFocusPolicy(GameInputFocusPolicy policy)
    {
        NativeInterface.SetFocusPolicy(policy);
    }

    /// <summary>
    ///     Creates an aggregate device for the specified <paramref name="inputKind" /> and returns its identifier.
    /// </summary>
    public AppLocalDeviceId CreateAggregateDevice(GameInputKind inputKind)
    {
        var hr = NativeInterface.CreateAggregateDevice(inputKind, out var deviceId);
        GameInputException.ThrowIfFailed(hr, "IGameInput.CreateAggregateDevice failed.");

        return deviceId;
    }

    /// <summary>
    ///     Disables a previously created aggregate device.
    /// </summary>
    public void DisableAggregateDevice(AppLocalDeviceId deviceId)
    {
        var hr = NativeInterface.DisableAggregateDevice(deviceId);
        GameInputException.ThrowIfFailed(hr, "IGameInput.DisableAggregateDevice failed.");
    }

    /// <summary>
    ///     Enumerates the current set of devices matching the provided filter.
    /// </summary>
    /// <remarks>
    ///     The returned <see cref="GameInputDevice" /> instances are disposable projections over native COM interfaces.
    ///     Callers are responsible for disposing each device after use to release the underlying unmanaged references.
    /// </remarks>
    public IReadOnlyList<GameInputDevice> EnumerateDevices(
        GameInputKind inputKind,
        GameInputDeviceStatus statusFilter = GameInputDeviceStatus.Connected)
    {
        using var collector = new DeviceEnumerationCollector();
        collector.Enumerate(NativeInterface, inputKind, statusFilter);
        return collector.ToArray();
    }

    private unsafe sealed class DeviceEnumerationCollector : IDisposable
    {
        private static readonly GameInputDeviceCallback Callback = OnDeviceEnumerated;

        private readonly List<GameInputDevice> _devices = new();
        private GCHandle _handle;

        public DeviceEnumerationCollector()
        {
            _handle = GCHandle.Alloc(this, GCHandleType.Normal);
        }

        public unsafe void Enumerate(IGameInput gameInput, GameInputKind kind, GameInputDeviceStatus statusFilter)
        {
            ArgumentNullException.ThrowIfNull(gameInput);

            var contextPtr = (void*)GCHandle.ToIntPtr(_handle);
            var hr = gameInput.RegisterDeviceCallback(
                device: null,
                inputKind: kind,
                statusFilter: statusFilter,
                enumerationKind: GameInputEnumerationKind.Blocking,
                context: contextPtr,
                callback: Callback,
                callbackToken: out _);

            GameInputException.ThrowIfFailed(hr, "IGameInput.RegisterDeviceCallback failed.");

            GC.KeepAlive(gameInput);
        }

        public GameInputDevice[] ToArray() => _devices.ToArray();

        private static unsafe void OnDeviceEnumerated(
            ulong callbackToken,
            void* context,
            IGameInputDevice device,
            ulong timestamp,
            GameInputDeviceStatus currentStatus,
            GameInputDeviceStatus previousStatus)
        {
            var handle = GCHandle.FromIntPtr((nint)context);
            if (handle.Target is not DeviceEnumerationCollector collector)
            {
                return;
            }

            var deviceHandle = GameInputDeviceHandle.FromInterface(device);
            var wrapper = new GameInputDevice(deviceHandle, timestamp, currentStatus, previousStatus);
            collector._devices.Add(wrapper);
        }

        public void Dispose()
        {
            if (_handle.IsAllocated)
            {
                _handle.Free();
            }
        }
    }
}
