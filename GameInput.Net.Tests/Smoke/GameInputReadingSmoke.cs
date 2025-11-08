using System;
using System.Runtime.Versioning;
using System.Threading;
using GameInputDotNet;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Tests.Infrastructure;
using Xunit;

namespace GameInputDotNet.Tests.Smoke;

public sealed class GameInputReadingSmoke
{
    private static readonly GameInputKind[] ProbeKinds =
    {
        GameInputKind.Controller,
        GameInputKind.Gamepad,
        GameInputKind.Keyboard,
        GameInputKind.Mouse,
        GameInputKind.Sensors,
        GameInputKind.ArcadeStick,
        GameInputKind.FlightStick,
        GameInputKind.RacingWheel
    };

    [WindowsOnlySkippableFact]
    [SupportedOSPlatform("windows")]
    public void CurrentReading_AllowsSnapshotAccess()
    {
        using var gameInput = GameInputFactory.Create();
        var exercised = false;

        foreach (var kind in ProbeKinds)
        {
            GameInputReading? reading = null;
            try
            {
                reading = gameInput.GetCurrentReading(kind);
            }
            catch (GameInputException ex) when (ShouldSkip(ex))
            {
                continue;
            }

            if (reading is null)
            {
                continue;
            }

            using (reading)
            {
                exercised = true;

                _ = reading.GetInputKind();
                _ = reading.GetTimestamp();

                GameInputDevice? readingDevice = null;
                try
                {
                    readingDevice = reading.GetDevice();
                    using (readingDevice)
                    {
                        _ = readingDevice.GetCurrentStatus();
                    }
                }
                catch (GameInputException ex) when (ShouldSkip(ex))
                {
                    readingDevice?.Dispose();
                }

                _ = reading.GetControllerState();
                _ = reading.GetKeyboardState();
                _ = reading.GetMouseState();
                _ = reading.GetSensorsState();
                _ = reading.GetArcadeStickState();
                _ = reading.GetFlightStickState();
                _ = reading.GetGamepadState();
                _ = reading.GetRacingWheelState();

                var rawReport = reading.GetRawReport();
                if (rawReport is not null)
                {
                    using (rawReport)
                    {
                        _ = rawReport.GetReportInfo();
                        _ = rawReport.GetRawData();
                    }
                }
            }
        }

        if (!exercised)
        {
            Console.WriteLine("Skipping reading snapshot smoke: no readings returned for probed kinds.");
            Skip.If(true, "GameInput.GetCurrentReading returned no usable snapshots. Connect hardware that produces readings for at least one probed kind.");
        }
    }

    [WindowsOnlySkippableFact]
    [SupportedOSPlatform("windows")]
    public void ReadingTraversal_PreviousAndNextDoNotThrow()
    {
        using var gameInput = GameInputFactory.Create();
        var exercised = false;

        foreach (var kind in ProbeKinds)
        {
            GameInputReading? current = null;
            try
            {
                current = gameInput.GetCurrentReading(kind);
            }
            catch (GameInputException ex) when (ShouldSkip(ex))
            {
                continue;
            }

            if (current is null)
            {
                continue;
            }

            using (current)
            {
                exercised = true;

                GameInputReading? next = null;
                GameInputReading? previous = null;
                try
                {
                    next = gameInput.GetNextReading(current, kind);
                }
                catch (GameInputException ex) when (ShouldSkip(ex))
                {
                }
                finally
                {
                    next?.Dispose();
                }

                try
                {
                    previous = gameInput.GetPreviousReading(current, kind);
                }
                catch (GameInputException ex) when (ShouldSkip(ex))
                {
                }
                finally
                {
                    previous?.Dispose();
                }
            }
        }

        if (!exercised)
        {
            Console.WriteLine("Skipping reading traversal smoke: no readings available to traverse.");
            Skip.If(true, "Reading traversal could not be exercised. Ensure at least one reading is available to walk via GetNextReading/GetPreviousReading.");
        }
    }

    [WindowsOnlySkippableFact]
    [SupportedOSPlatform("windows")]
    public void ReadingCallbacks_RegisterAndDispose()
    {
        using var gameInput = GameInputFactory.Create();

        using var registration = gameInput.RegisterReadingCallback(
            device: null,
            inputKind: GameInputKind.Controller | GameInputKind.Gamepad | GameInputKind.Keyboard | GameInputKind.Mouse,
            handler: reading =>
            {
                try
                {
                    reading.Dispose();
                }
                catch
                {
                    // Ignore disposal issues inside smoke callback; registration disposal handles cleanup.
                }
            });

        Thread.Sleep(10);
    }

    private static bool ShouldSkip(GameInputException ex)
    {
        if (ex is GameInputDeviceNotConnectedException
            or GameInputDeviceNotFoundException
            or GameInputHapticInfoNotFoundException
            or GameInputFeedbackNotSupportedException
            or GameInputAggregateOperationNotSupportedException
            or GameInputInputKindNotPresentException)
        {
            return true;
        }

        if (ex.Error is GameInputErrorCode.ReadingNotFound or GameInputErrorCode.ReferenceReadingTooOld)
        {
            return true;
        }

        return ex.ErrorCode == unchecked((int)0x80004001)
            || ex.ErrorCode == unchecked((int)0x80070032);
    }
}
