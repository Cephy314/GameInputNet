using System;
using System.Linq;
using System.Runtime.Versioning;
using GameInputDotNet;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Tests.Infrastructure;
using Microsoft.Win32.SafeHandles;
using Xunit;

namespace GameInputDotNet.Tests.Smoke;

public sealed class GameInputDispatcherMapperSmoke
{
    [WindowsOnlySkippableFact]
    [SupportedOSPlatform("windows")]
    public void Dispatcher_CreateDispatchAndOpenHandle()
    {
        using var gameInput = GameInputFactory.Create();

        using var dispatcher = gameInput.CreateDispatcher();
        dispatcher.Dispatch(TimeSpan.Zero);
        dispatcher.Dispatch(0);

        try
        {
            using SafeWaitHandle waitHandle = dispatcher.OpenWaitHandle();
            Assert.False(waitHandle.IsInvalid);
        }
        catch (GameInputException ex) when (IsNotImplemented(ex))
        {
            // Some redistributables may not expose wait handles; accept as skip.
        }
    }

    [WindowsOnlySkippableFact]
    [SupportedOSPlatform("windows")]
    public void DeviceMapper_CreateAndQueryMappings()
    {
        using var gameInput = GameInputFactory.Create();
        var exercised = false;

        foreach (var kind in new[]
                 {
                     GameInputKind.Gamepad,
                     GameInputKind.Controller,
                     GameInputKind.RacingWheel,
                     GameInputKind.FlightStick,
                     GameInputKind.ArcadeStick
                 })
        {
            var devices = gameInput.EnumerateDevices(kind).ToArray();
            if (devices.Length == 0)
            {
                continue;
            }

            foreach (var device in devices)
            {
                using var candidate = device;
                try
                {
                    using var mapper = candidate.CreateInputMapper();

                    mapper.TryGetGamepadButtonMapping(GameInputGamepadButtons.A, out _);
                    mapper.TryGetGamepadAxisMapping(GameInputGamepadAxes.LeftTrigger, out _);
                    mapper.TryGetArcadeStickButtonMapping(GameInputArcadeStickButtons.Action1, out _);
                    mapper.TryGetFlightStickAxisMapping(GameInputFlightStickAxes.Roll, out _);
                    mapper.TryGetFlightStickButtonMapping(GameInputFlightStickButtons.FirePrimary, out _);
                    mapper.TryGetRacingWheelAxisMapping(GameInputRacingWheelAxes.Throttle, out _);
                    mapper.TryGetRacingWheelButtonMapping(GameInputRacingWheelButtons.A, out _);
                    exercised = true;
                    break;
                }
                catch (GameInputException ex) when (IsMapperUnsupported(ex))
                {
                    continue;
                }
            }

            if (exercised)
            {
                break;
            }
        }

        if (!exercised)
        {
            Console.WriteLine("Skipping input mapper smoke: no mapping metadata exposed by devices.");
            Skip.If(true, "Input mapper helpers were not exercised. Connect hardware that exposes mapping metadata.");
        }
    }

    private static bool IsNotImplemented(GameInputException ex)
    {
        return ex.ErrorCode == unchecked((int)0x80004001);
    }

    private static bool IsMapperUnsupported(GameInputException ex)
    {
        return ex is GameInputDeviceNotConnectedException
               or GameInputDeviceNotFoundException
               || ex.ErrorCode == unchecked((int)0x80004001)
               || ex.Error is GameInputErrorCode.FeedbackNotSupported
               || ex.Error is GameInputErrorCode.AggregateOperationNotSupported
               || ex.Error is GameInputErrorCode.InputKindNotPresent;
    }
}
