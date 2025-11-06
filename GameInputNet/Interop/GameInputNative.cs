using System.Runtime.InteropServices;

namespace GameInputNet.Interop;

public static class GameInputNative
{
    private const CallingConvention CallbackConvention = CallingConvention.StdCall;

    [UnmanagedFunctionPointer(CallbackConvention)]
    public delegate void GameInputDeviceCallback(ulong callbackToken, IntPtr context,
        [MarshalAs(UnmanagedType.Interface)] IGameInputDevice device, ulong timestamp,
        GameInputDeviceStatus currentStatus, GameInputDeviceStatus
            previousStatus);

    [UnmanagedFunctionPointer(CallbackConvention)]
    public delegate void GameInputKeyboardLayoutCallback(ulong callbackToken, IntPtr context,
        [MarshalAs(UnmanagedType.Interface)] IGameInputDevice device, ulong timestamp, uint currentLayout, uint
            previousLayout);

    [UnmanagedFunctionPointer(CallbackConvention)]
    public delegate void GameInputReadingCallback(ulong callbackToken, IntPtr context,
        [MarshalAs(UnmanagedType.Interface)] IGameInputReading reading);

    [UnmanagedFunctionPointer(CallbackConvention)]
    public delegate void GameInputSystemButtonCallback(ulong callbackToken, IntPtr context,
        [MarshalAs(UnmanagedType.Interface)] IGameInputDevice device, ulong timestamp,
        GameInputSystemButtons currentButtons, GameInputSystemButtons
            previousButtons);

    private const string GameInputDll = "GameInput.dll";

    [DllImport(GameInputDll, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static extern int GameInputCreate(out IGameInput? gameInput);


    [ComImport]
    [Guid("20EFC1C7-5D9A-43BA-B26F-B807FA48609C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IGameInput
    {
        [PreserveSig]
        ulong GetCurrentTimestamp();

        [PreserveSig]
        int GetCurrentReading(GameInputKind inputKind, [MarshalAs(UnmanagedType.Interface)] IGameInputDevice? device,
            out IGameInputReading? reading);

        [PreserveSig]
        int GetNextReading([MarshalAs(UnmanagedType.Interface)] IGameInputReading referenceReading,
            GameInputKind inputKind,
            [MarshalAs(UnmanagedType.Interface)] IGameInputDevice? device, out IGameInputReading? reading);

        [PreserveSig]
        int GetPreviousReading([MarshalAs(UnmanagedType.Interface)] IGameInputReading referenceReading, GameInputKind
            inputKind, [MarshalAs(UnmanagedType.Interface)] IGameInputDevice? device, out IGameInputReading? reading);

        [PreserveSig]
        int RegisterReadingCallback([MarshalAs(UnmanagedType.Interface)] IGameInputDevice? device,
            GameInputKind inputKind,
            IntPtr context, GameInputReadingCallback callback, out ulong callbackToken);

        [PreserveSig]
        int RegisterDeviceCallback([MarshalAs(UnmanagedType.Interface)] IGameInputDevice? device,
            GameInputKind inputKind, uint statusFilter, GameInputEnumerationKind enumerationKind, IntPtr context,
            GameInputDeviceCallback callback, out ulong callbackToken);

        [PreserveSig]
        int RegisterSystemButtonCallback([MarshalAs(UnmanagedType.Interface)] IGameInputDevice? device,
            GameInputSystemButtons
                buttonFilter, IntPtr context, GameInputSystemButtonCallback callback, out ulong callbackToken);

        [PreserveSig]
        int RegisterKeyboardLayoutCallback([MarshalAs(UnmanagedType.Interface)] IGameInputDevice? device, IntPtr
            context, GameInputKeyboardLayoutCallback callback, out ulong callbackToken);

        [PreserveSig]
        void StopCallback(ulong callbackToken);

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool UnregisterCallback(ulong callbackToken);

        [PreserveSig]
        int CreateDispatcher(out IGameInputDispatcher? dispatcher);

        [PreserveSig]
        int FindDeviceFromId(ref Guid deviceId, out IGameInputDevice? device);

        [PreserveSig]
        int FindDeviceFromPlatformString([MarshalAs(UnmanagedType.LPWStr)] string value, out IGameInputDevice?
            device);

        [PreserveSig]
        void SetFocusPolicy(GameInputFocusPolicy policy);

        [PreserveSig]
        int CreateAggregateDevice(GameInputKind inputKind, out Guid deviceId);

        [PreserveSig]
        int DisableAggregateDevice(ref Guid deviceId);
    }

    [ComImport]
    [Guid("C81C4CDE-ED1A-4631-A30F-C556A6241A1F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IGameInputReading
    {
    }

    [ComImport]
    [Guid("63E2F38B-A399-4275-8AE7-D4C6E524D12A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IGameInputDevice
    {
    }

    [ComImport]
    [Guid("415EED2E-98CB-42C2-8F28-B94601074E31")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IGameInputDispatcher
    {
    }
}
