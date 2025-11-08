using System;
using System.Runtime.Versioning;
using GameInputDotNet;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Tests.Infrastructure;
using Xunit;

namespace GameInputDotNet.Tests.Smoke;

public sealed class GameInputAggregateDeviceSmoke
{
    [WindowsOnlySkippableFact]
    [SupportedOSPlatform("windows")]
    public void CreateAndDisableAggregateDevice_Succeeds()
    {
        using var gameInput = GameInputFactory.Create();
        var exercised = false;

        var blackList = new[]
        {
            GameInputKind.Controller,
            GameInputKind.ControllerAxis,
            GameInputKind.ControllerButton,
            GameInputKind.ControllerSwitch,
            GameInputKind.Sensors,
            GameInputKind.Unknown,
            GameInputKind.RawDeviceReport
        };

        foreach (var kind in Enum.GetValues<GameInputKind>())
        {
            if (Array.IndexOf(blackList, kind) >= 0)
                continue;

            try
            {
                var deviceId = gameInput.CreateAggregateDevice(kind);
                gameInput.DisableAggregateDevice(deviceId);
                exercised = true;
            }
            catch (GameInputException ex) when (IsAggregateUnsupported(ex))
            {
                continue;
            }
        }

        if (!exercised)
        {
            Console.WriteLine("Skipping aggregate device smoke: aggregate creation/disable not supported on this configuration.");
            Skip.If(true, "Aggregate device creation/disable was not exercised. Ensure the redistributable supports aggregate devices or provide hardware that exposes them.");
        }
    }

    private static bool IsAggregateUnsupported(GameInputException ex)
    {
        return ex.Error is GameInputErrorCode.AggregateOperationNotSupported
            || ex.ErrorCode == unchecked((int)0x80004001);
        // E_NOTIMPL on older redistributables
    }
}
