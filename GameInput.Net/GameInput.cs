using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using GameInputDotNet.Interop;
using GameInputDotNet.Interop.Delegates;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Handles;
using GameInputDotNet.Interop.Interfaces;
using static GameInputDotNet.Interop.GameInputNative;

namespace GameInputDotNet;

/// <summary>
///     Managed wrapper over the native <c>IGameInput</c> interface.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed unsafe class GameInput : IDisposable
{
    public delegate void GameInputDeviceHandler(GameInputDevice device, ulong timestamp,
        GameInputDeviceStatus currentStatus, GameInputDeviceStatus previousStatus);

    public delegate void GameInputKeyboardLayoutHandler(GameInputDevice device, ulong timestamp, uint currentLayout,
        uint previousLayout);

    public delegate void GameInputReadingHandler(GameInputReading reading);

    public delegate void GameInputSystemButtonHandler(GameInputDevice device, ulong timestamp,
        GameInputSystemButtons currentButtons, GameInputSystemButtons previousButtons);

    private static readonly GameInputReadingCallback ReadingCallbackThunk = OnReadingCallback;
    private static readonly GameInputDeviceCallback DeviceCallbackThunk = OnDeviceCallback;
    private static readonly GameInputSystemButtonCallback SystemButtonCallbackThunk = OnSystemButtonCallback;
    private static readonly GameInputKeyboardLayoutCallback KeyboardLayoutCallbackThunk = OnKeyboardLayoutCallback;

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

    public static GameInput Create()
    {
        var hr = GameInputCreate(out var gameInput);
        GameInputException.ThrowIfFailed(hr, "GameInputCreate failed.");

        if (gameInput is null) throw new GameInputException("GameInputCreate returned a null interface.", hr);

        var handle = GameInputHandle.FromInterface(gameInput);
        return new GameInput(handle);
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
    ///     Creates a dispatcher that can be used to pump callbacks.
    /// </summary>
    public GameInputDispatcher CreateDispatcher()
    {
        var hr = NativeInterface.CreateDispatcher(out var dispatcher);
        GameInputErrorMapper.ThrowIfFailed(hr, "IGameInput.CreateDispatcher failed.");

        if (dispatcher is null)
            throw new GameInputException("IGameInput.CreateDispatcher returned a null interface.", hr);

        var handle = GameInputDispatcherHandle.FromInterface(dispatcher);
        GC.KeepAlive(this);
        return new GameInputDispatcher(handle);
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
    ///     Returns the latest reading for the specified input kind.
    /// </summary>
    public GameInputReading? GetCurrentReading(GameInputKind inputKind, GameInputDevice? device = null)
    {
        var hr = NativeInterface.GetCurrentReading(inputKind, device?.NativeInterface, out var reading);
        GameInputException.ThrowIfFailed(hr, "IGameInput.GetCurrentReading failed.");

        return ConvertReadingFromNative(reading);
    }

    /// <summary>
    ///     Returns the next reading relative to <paramref name="referenceReading" />.
    /// </summary>
    public GameInputReading? GetNextReading(GameInputReading referenceReading, GameInputKind inputKind,
        GameInputDevice? device = null)
    {
        ArgumentNullException.ThrowIfNull(referenceReading);

        var hr = NativeInterface.GetNextReading(referenceReading.NativeInterface, inputKind,
            device?.NativeInterface, out var reading);
        GameInputException.ThrowIfFailed(hr, "IGameInput.GetNextReading failed.");

        return ConvertReadingFromNative(reading);
    }

    /// <summary>
    ///     Returns the previous reading relative to <paramref name="referenceReading" />.
    /// </summary>
    public GameInputReading? GetPreviousReading(GameInputReading referenceReading, GameInputKind inputKind,
        GameInputDevice? device = null)
    {
        ArgumentNullException.ThrowIfNull(referenceReading);

        var hr = NativeInterface.GetPreviousReading(referenceReading.NativeInterface, inputKind,
            device?.NativeInterface, out var reading);
        GameInputException.ThrowIfFailed(hr, "IGameInput.GetPreviousReading failed.");

        return ConvertReadingFromNative(reading);
    }

    public GameInputCallbackRegistration RegisterReadingCallback(GameInputDevice? device, GameInputKind inputKind,
        GameInputReadingHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var context = GCHandle.Alloc(new ReadingCallbackContext(this, handler));
        ulong token = 0;
        try
        {
            var contextPtr = (void*)GCHandle.ToIntPtr(context);
            var hr = NativeInterface.RegisterReadingCallback(device?.NativeInterface, inputKind, contextPtr,
                ReadingCallbackThunk, out token);
            GameInputException.ThrowIfFailed(hr, "IGameInput.RegisterReadingCallback failed.");
            return new GameInputCallbackRegistration(this, token, context);
        }
        catch
        {
            if (token != 0)
            {
                NativeInterface.StopCallback(token);
                NativeInterface.UnregisterCallback(token);
            }

            if (context.IsAllocated) context.Free();
            throw;
        }
        finally
        {
            GC.KeepAlive(device);
        }
    }

    public GameInputCallbackRegistration RegisterDeviceCallback(GameInputDevice? device, GameInputKind inputKind,
        GameInputDeviceStatus statusFilter, GameInputEnumerationKind enumerationKind, GameInputDeviceHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var context = GCHandle.Alloc(new DeviceCallbackContext(this, handler));
        ulong token = 0;
        try
        {
            var contextPtr = (void*)GCHandle.ToIntPtr(context);
            var hr = NativeInterface.RegisterDeviceCallback(device?.NativeInterface, inputKind, statusFilter,
                enumerationKind,
                contextPtr, DeviceCallbackThunk, out token);
            GameInputException.ThrowIfFailed(hr, "IGameInput.RegisterDeviceCallback failed.");
            return new GameInputCallbackRegistration(this, token, context);
        }
        catch
        {
            if (token != 0)
            {
                NativeInterface.StopCallback(token);
                NativeInterface.UnregisterCallback(token);
            }

            if (context.IsAllocated) context.Free();
            throw;
        }
        finally
        {
            GC.KeepAlive(device);
        }
    }

    public GameInputCallbackRegistration RegisterSystemButtonCallback(GameInputDevice? device,
        GameInputSystemButtons buttonFilter, GameInputSystemButtonHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var context = GCHandle.Alloc(new SystemButtonCallbackContext(this, handler));
        ulong token = 0;
        try
        {
            var contextPtr = (void*)GCHandle.ToIntPtr(context);
            var hr = NativeInterface.RegisterSystemButtonCallback(device?.NativeInterface, buttonFilter, contextPtr,
                SystemButtonCallbackThunk, out token);
            GameInputException.ThrowIfFailed(hr, "IGameInput.RegisterSystemButtonCallback failed.");
            return new GameInputCallbackRegistration(this, token, context);
        }
        catch
        {
            if (token != 0)
            {
                NativeInterface.StopCallback(token);
                NativeInterface.UnregisterCallback(token);
            }

            if (context.IsAllocated) context.Free();
            throw;
        }
        finally
        {
            GC.KeepAlive(device);
        }
    }

    public GameInputCallbackRegistration RegisterKeyboardLayoutCallback(GameInputDevice? device,
        GameInputKeyboardLayoutHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var context = GCHandle.Alloc(new KeyboardLayoutCallbackContext(this, handler));
        ulong token = 0;
        try
        {
            var contextPtr = (void*)GCHandle.ToIntPtr(context);
            var hr = NativeInterface.RegisterKeyboardLayoutCallback(device?.NativeInterface, contextPtr,
                KeyboardLayoutCallbackThunk, out token);
            GameInputException.ThrowIfFailed(hr, "IGameInput.RegisterKeyboardLayoutCallback failed.");
            return new GameInputCallbackRegistration(this, token, context);
        }
        catch
        {
            if (token != 0)
            {
                NativeInterface.StopCallback(token);
                NativeInterface.UnregisterCallback(token);
            }

            if (context.IsAllocated) context.Free();
            throw;
        }
        finally
        {
            GC.KeepAlive(device);
        }
    }

    /// <summary>
    ///     Retrieves a single device by its <see cref="AppLocalDeviceId" />.
    /// </summary>
    public GameInputDevice FindDeviceById(AppLocalDeviceId deviceId)
    {
        var hr = NativeInterface.FindDeviceFromId(in deviceId, out var device);
        GameInputException.ThrowIfFailed(hr, "IGameInput.FindDeviceFromId failed.");

        return ConvertDeviceFromNative(device, hr, "IGameInput.FindDeviceFromId returned a null interface.");
    }

    /// <summary>
    ///     Retrieves a single device matching the provided platform string.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This method is untested due to lack of documentation and inability to verify output.
    ///         historically the API returned not implemented so it should be considered unreliable.
    ///         If you have more info please contact the developers of this wrapper.
    ///     </para>
    /// </remarks>
    public GameInputDevice FindDeviceFromPlatformString(string platformString)
    {
        var hr = NativeInterface.FindDeviceFromPlatformString(platformString, out var device);
        GameInputException.ThrowIfFailed(hr, "IGameInput.FindDeviceFromPlatformString failed.");

        return ConvertDeviceFromNative(device, hr,
            "IGameInput.FindDeviceFromPlatformString returned a null interface.");
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

    private GameInputDevice ConvertDeviceFromNative(IGameInputDevice? device, int hresult, string failureMessage)
    {
        if (device is null)
            throw new GameInputException(failureMessage, hresult);

        var status = device.GetDeviceStatus();
        var timestamp = NativeInterface.GetCurrentTimestamp();
        var handle = GameInputDeviceHandle.FromInterface(device);
        return new GameInputDevice(handle, timestamp, status, status);
    }

    private static GameInputDevice WrapDevice(IGameInputDevice device, ulong timestamp,
        GameInputDeviceStatus currentStatus, GameInputDeviceStatus previousStatus)
    {
        var handle = GameInputDeviceHandle.FromInterface(device);
        return new GameInputDevice(handle, timestamp, currentStatus, previousStatus);
    }

    private static GameInputReading? ConvertReadingFromNative(IGameInputReading? reading)
    {
        if (reading is null) return null;
        var handle = GameInputReadingHandle.FromInterface(reading);
        return new GameInputReading(handle);
    }

    private static void OnReadingCallback(ulong callbackToken, void* context, IGameInputReading reading)
    {
        var handle = GCHandle.FromIntPtr((nint)context);
        if (handle.Target is not ReadingCallbackContext ctx) return;

        var managed = ConvertReadingFromNative(reading);
        if (managed is null) return;

        using (managed)
        {
            ctx.Handler(managed);
        }
    }

    private static void OnDeviceCallback(ulong callbackToken, void* context, IGameInputDevice device,
        ulong timestamp, GameInputDeviceStatus currentStatus, GameInputDeviceStatus previousStatus)
    {
        var handle = GCHandle.FromIntPtr((nint)context);
        if (handle.Target is not DeviceCallbackContext ctx) return;

        var managedDevice = WrapDevice(device, timestamp, currentStatus, previousStatus);
        try
        {
            ctx.Handler(managedDevice, timestamp, currentStatus, previousStatus);
        }
        finally
        {
            managedDevice.Dispose();
        }
    }

    private static void OnSystemButtonCallback(ulong callbackToken, void* context, IGameInputDevice device,
        ulong timestamp, GameInputSystemButtons currentButtons, GameInputSystemButtons previousButtons)
    {
        var handle = GCHandle.FromIntPtr((nint)context);
        if (handle.Target is not SystemButtonCallbackContext ctx) return;

        var status = device.GetDeviceStatus();
        var managedDevice = WrapDevice(device, timestamp, status, status);
        try
        {
            ctx.Handler(managedDevice, timestamp, currentButtons, previousButtons);
        }
        finally
        {
            managedDevice.Dispose();
        }
    }

    private static void OnKeyboardLayoutCallback(ulong callbackToken, void* context, IGameInputDevice device,
        ulong timestamp, uint currentLayout, uint previousLayout)
    {
        var handle = GCHandle.FromIntPtr((nint)context);
        if (handle.Target is not KeyboardLayoutCallbackContext ctx) return;

        var status = device.GetDeviceStatus();
        var managedDevice = WrapDevice(device, timestamp, status, status);
        try
        {
            ctx.Handler(managedDevice, timestamp, currentLayout, previousLayout);
        }
        finally
        {
            managedDevice.Dispose();
        }
    }

    private sealed class ReadingCallbackContext
    {
        public ReadingCallbackContext(GameInput owner, GameInputReadingHandler handler)
        {
            Owner = owner;
            Handler = handler;
        }

        public GameInput Owner { get; }
        public GameInputReadingHandler Handler { get; }
    }

    private sealed class DeviceCallbackContext
    {
        public DeviceCallbackContext(GameInput owner, GameInputDeviceHandler handler)
        {
            Owner = owner;
            Handler = handler;
        }

        public GameInput Owner { get; }
        public GameInputDeviceHandler Handler { get; }
    }

    private sealed class SystemButtonCallbackContext
    {
        public SystemButtonCallbackContext(GameInput owner, GameInputSystemButtonHandler handler)
        {
            Owner = owner;
            Handler = handler;
        }

        public GameInput Owner { get; }
        public GameInputSystemButtonHandler Handler { get; }
    }

    private sealed class KeyboardLayoutCallbackContext
    {
        public KeyboardLayoutCallbackContext(GameInput owner, GameInputKeyboardLayoutHandler handler)
        {
            Owner = owner;
            Handler = handler;
        }

        public GameInput Owner { get; }
        public GameInputKeyboardLayoutHandler Handler { get; }
    }

    private sealed class DeviceEnumerationCollector : IDisposable
    {
        private static readonly GameInputDeviceCallback Callback = OnDeviceEnumerated;

        private readonly List<GameInputDevice> _devices = new();
        private GCHandle _handle;

        public DeviceEnumerationCollector()
        {
            _handle = GCHandle.Alloc(this, GCHandleType.Normal);
        }

        public void Dispose()
        {
            if (_handle.IsAllocated) _handle.Free();
        }

        public void Enumerate(IGameInput gameInput, GameInputKind kind, GameInputDeviceStatus statusFilter)
        {
            ArgumentNullException.ThrowIfNull(gameInput);

            var contextPtr = (void*)GCHandle.ToIntPtr(_handle);
            var hr = gameInput.RegisterDeviceCallback(
                null,
                kind,
                statusFilter,
                GameInputEnumerationKind.Blocking,
                contextPtr,
                Callback,
                out _);

            GameInputException.ThrowIfFailed(hr, "IGameInput.RegisterDeviceCallback failed.");

            GC.KeepAlive(gameInput);
        }

        public GameInputDevice[] ToArray()
        {
            return _devices.ToArray();
        }

        private static void OnDeviceEnumerated(
            ulong callbackToken,
            void* context,
            IGameInputDevice device,
            ulong timestamp,
            GameInputDeviceStatus currentStatus,
            GameInputDeviceStatus previousStatus)
        {
            var handle = GCHandle.FromIntPtr((nint)context);
            if (handle.Target is not DeviceEnumerationCollector collector) return;

            var wrapper = WrapDevice(device, timestamp, currentStatus, previousStatus);
            collector._devices.Add(wrapper);
        }
    }

    public sealed class GameInputCallbackRegistration : IDisposable
    {
        private readonly GameInput _owner;
        private readonly ulong _token;
        private GCHandle _context;
        private bool _disposed;

        internal GameInputCallbackRegistration(GameInput owner, ulong token, GCHandle context)
        {
            _owner = owner;
            _token = token;
            _context = context;
        }

        public void Dispose()
        {
            if (_disposed) return;

            if (_context.IsAllocated) _context.Free();

            try
            {
                _owner.NativeInterface.StopCallback(_token);
            }
            finally
            {
                _owner.NativeInterface.UnregisterCallback(_token);
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        ~GameInputCallbackRegistration()
        {
            Dispose();
        }
    }
}