using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using GameInputDotNet;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Tests.Infrastructure;

namespace GameInputDotNet.Tests.Smoke;

public sealed class GameInputAggregateDeviceSmoke
{
    [WindowsOnlyFact]
    [SupportedOSPlatform("windows")]
    public void CreateAndDisableAggregateDevice_Succeeds()
    {
        using var gameInput = GameInputFactory.Create();
        var dict = new Dictionary<GameInputKind, bool>();
        var blackList = new List<GameInputKind>
        {
            GameInputKind.Controller, GameInputKind.ControllerAxis, GameInputKind.ControllerButton,
            GameInputKind.ControllerSwitch, GameInputKind.Sensors, GameInputKind.Unknown, GameInputKind.RawDeviceReport
        };
        foreach (var kind in Enum.GetValues<GameInputKind>())
        {
            if (blackList.Contains(kind))
                // Not supported Kind
                continue;

            try
            {
                var deviceId = gameInput.CreateAggregateDevice(kind);
                gameInput.DisableAggregateDevice(deviceId);
                dict.Add(kind, true);
            }
            catch (GameInputException ex) when (IsAggregateUnsupported(ex))
            {
                dict.Add(kind, false);
            }
        }

        if (dict.Count == 0) throw new Exception("Failed smoke test. No aggregate devices found is unexpected.");
    }

    private static bool IsAggregateUnsupported(GameInputException ex)
    {
        return ex.ErrorCode is unchecked((int)0x838A000B) // GAMEINPUT_E_AGGREGATE_OPERATION_NOT_SUPPORTED
            or unchecked((int)0x80004001);
        // E_NOTIMPL on older redistributables
    }
}
