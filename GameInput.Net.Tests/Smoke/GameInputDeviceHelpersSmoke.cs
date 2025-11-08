using System;
using System.Runtime.Versioning;
using GameInputDotNet;
using GameInputDotNet.Interop;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Structs;
using GameInputDotNet.Tests.Infrastructure;
using Xunit;

namespace GameInputDotNet.Tests.Smoke;

public sealed class GameInputDeviceHelpersSmoke
{
    private static readonly GameInputKind[] ProbeKinds =
    {
        GameInputKind.Controller,
        GameInputKind.Gamepad,
        GameInputKind.RacingWheel,
        GameInputKind.FlightStick,
        GameInputKind.ArcadeStick
    };

    [WindowsOnlySkippableFact]
    [SupportedOSPlatform("windows")]
    public void GetHapticInfo_WhenAvailable_AllowsMetadataQueries()
    {
        using var gameInput = GameInputFactory.Create();
        var exercised = false;

        foreach (var kind in ProbeKinds)
        {
            foreach (var device in gameInput.EnumerateDevices(kind))
            {
                using var candidate = device;
                try
                {
                    var info = candidate.GetHapticInfo();
                    Assert.InRange(info.LocationCount, 0u, 8u);
                    _ = info.GetAudioEndpointId();
                    var locations = info.GetLocations();
                    Assert.True(locations.Length <= info.LocationCount);
                    exercised = true;
                    break;
                }
                catch (GameInputException ex) when (ShouldSkip(ex))
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
            Console.WriteLine("Skipping haptics metadata smoke: no compatible devices detected.");
            Skip.If(true, "No devices exposed haptic metadata. Connect hardware with haptic support or investigate the redistributable capabilities.");
        }
    }

    [WindowsOnlySkippableFact]
    [SupportedOSPlatform("windows")]
    public void ForceFeedbackHelpers_CreateEffectAndAdjustGain()
    {
        using var gameInput = GameInputFactory.Create();
        var exercised = false;

        foreach (var kind in ProbeKinds)
        {
            foreach (var device in gameInput.EnumerateDevices(kind))
            {
                using var candidate = device;
                try
                {
                    var info = candidate.GetDeviceInfo();
                    if (info.ForceFeedbackMotorCount == 0)
                    {
                        continue;
                    }

                    var parameters = CreateNoOpForceFeedbackParameters();
                    using var effect = candidate.CreateForceFeedbackEffect(0, parameters);
                    Assert.Equal(0u, effect.GetMotorIndex());
                    using var owningDevice = effect.GetDevice();
                    _ = owningDevice.GetCurrentStatus();

                    candidate.SetForceFeedbackMotorGain(0, 0.0f);
                    _ = candidate.IsForceFeedbackMotorPoweredOn(0);

                    effect.SetGain(0.0f);
                    _ = effect.GetGain();
                    effect.SetParameters(parameters);
                    var readBack = effect.GetParameters();
                    Assert.Equal(parameters.Kind, readBack.Kind);
                    effect.SetState(GameInputFeedbackEffectState.Stopped);

                    if (info.SupportedRumbleMotors != GameInputRumbleMotors.None)
                    {
                        candidate.SetRumbleState(default);
                    }

                    exercised = true;
                    break;
                }
                catch (GameInputException ex) when (ShouldSkip(ex))
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
            Console.WriteLine("Skipping force feedback smoke: no FF-capable devices available.");
            Skip.If(true, "Force feedback helpers were not exercised. Connect a force-feedback capable device and ensure the redistributable exposes motor APIs.");
        }
    }

    [WindowsOnlySkippableFact]
    [SupportedOSPlatform("windows")]
    public void RawDeviceReportHelpers_CreateAndSend()
    {
        using var gameInput = GameInputFactory.Create();
        var exercised = false;

        foreach (var kind in ProbeKinds)
        {
            foreach (var device in gameInput.EnumerateDevices(kind))
            {
                using var candidate = device;
                try
                {
                    var info = candidate.GetDeviceInfo();
                    var outputs = info.GetOutputReportInfo();
                    if (outputs.Length == 0)
                    {
                        continue;
                    }

                    var reportInfo = outputs[0];
                    if (reportInfo.Size > int.MaxValue)
                    {
                        continue;
                    }

                    using var report = candidate.CreateRawDeviceReport(reportInfo.Id, reportInfo.Kind);
                    var roundTrip = report.GetReportInfo();
                    Assert.Equal(reportInfo.Kind, roundTrip.Kind);
                    Assert.Equal(reportInfo.Id, roundTrip.Id);

                    var payload = reportInfo.Size == 0 ? Array.Empty<byte>() : new byte[reportInfo.Size];
                    report.SetRawData(payload);
                    candidate.SendRawDeviceOutput(report);
                    exercised = true;
                    break;
                }
                catch (GameInputException ex) when (ShouldSkip(ex))
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
            Console.WriteLine("Skipping raw device report smoke: no writable output reports available.");
            Skip.If(true, "Raw device report helpers were not exercised. Connect hardware that exposes writable raw output reports.");
        }
    }

    private static bool ShouldSkip(GameInputException ex)
    {
        return ex is GameInputHapticInfoNotFoundException
               or GameInputFeedbackNotSupportedException
               or GameInputDeviceNotConnectedException
               or GameInputDeviceNotFoundException
               or GameInputAggregateOperationNotSupportedException
               or GameInputInputKindNotPresentException
               || ex.ErrorCode == unchecked((int)0x80004001)
               || ex.ErrorCode == unchecked((int)0x80070032);
    }

    private static GameInputForceFeedbackParams CreateNoOpForceFeedbackParameters()
    {
        var envelope = new GameInputForceFeedbackEnvelope
        {
            SustainDuration = 1000,
            SustainGain = 0.0f,
            PlayCount = 1,
            RepeatDelay = 0
        };

        var magnitude = new GameInputForceFeedbackMagnitude
        {
            LinearX = 0.0f,
            LinearY = 0.0f,
            LinearZ = 0.0f,
            AngularX = 0.0f,
            AngularY = 0.0f,
            AngularZ = 0.0f,
            Normal = 0.0f
        };

        return GameInputForceFeedbackParams.CreateConstant(new GameInputForceFeedbackConstantParams
        {
            Envelope = envelope,
            Magnitude = magnitude
        });
    }
}
