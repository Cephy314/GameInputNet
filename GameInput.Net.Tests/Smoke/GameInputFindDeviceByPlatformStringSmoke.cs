using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using GameInputDotNet;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Tests.Infrastructure;

namespace GameInputDotNet.Tests.Smoke;

public sealed class GameInputFindDeviceByPlatformStringSmoke
{
    private static readonly IReadOnlyList<GameInputKind> ProbeKinds =
    [
        GameInputKind.Controller,
        GameInputKind.Gamepad,
        GameInputKind.Keyboard,
        GameInputKind.Mouse,
        GameInputKind.Sensors
    ];

    [WindowsOnlyFact]
    [SupportedOSPlatform("windows")]
    public void FindDeviceByPlatformString_RoundTripsExistingDevice()
    {
        using var gameInput = GameInputFactory.Create();

        var count = 0;
        foreach (var kind in ProbeKinds)
        {
            var enumerated = gameInput.EnumerateDevices(kind);
            if (enumerated.Count == 0) continue;

            var devices = enumerated.ToArray();
            var primary = devices[0];

            try
            {
                var info = primary.GetDeviceInfo();
                var pnpPath = info.GetPnpPath();
                if (string.IsNullOrWhiteSpace(pnpPath)) continue;

                using var found = gameInput.FindDeviceFromPlatformString(pnpPath);
                var foundInfo = found.GetDeviceInfo();

                Assert.True(info.DeviceId.AsSpan().SequenceEqual(foundInfo.DeviceId.AsSpan()),
                    "Device identifiers should match when round-tripping through FindDeviceFromPlatformString.");
                count++;
            }
            catch (GameInputException ex) when (IsLookupUnsupported(ex))
            {
                continue;
            }
            finally
            {
                foreach (var device in devices) device.Dispose();
            }

            return;
        }

        Console.WriteLine($"Successes: {count}");
    }

    private static bool IsLookupUnsupported(GameInputException ex)
    {
        return ex.ErrorCode is unchecked((int)0x80004001) // E_NOTIMPL
            or unchecked((int)0x80070490);
        // ERROR_NOT_FOUND for unsupported kinds
    }
}
