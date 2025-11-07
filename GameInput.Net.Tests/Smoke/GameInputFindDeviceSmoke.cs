using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using GameInputDotNet;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Tests.Infrastructure;
using Xunit;

namespace GameInputDotNet.Tests.Smoke;

public sealed class GameInputFindDeviceSmoke
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
    public void FindDeviceById_RoundTripsExistingDevice()
    {
        using var gameInput = GameInputFactory.Create();

        foreach (var kind in ProbeKinds)
        {
            var enumerated = gameInput.EnumerateDevices(kind);
            if (enumerated.Count == 0)
            {
                continue;
            }

            var devices = enumerated.ToArray();
            var primary = devices[0];

            try
            {
                var info = primary.GetDeviceInfo();
                using var found = gameInput.FindDeviceById(info.DeviceId);
                var foundInfo = found.GetDeviceInfo();

                Assert.True(info.DeviceId.AsSpan().SequenceEqual(foundInfo.DeviceId.AsSpan()),
                    "Device identifiers should match when round-tripping through FindDeviceById.");
            }
            catch (GameInputException ex) when (ex.ErrorCode == unchecked((int)0x80004001))
            {
                // Redistributable may not implement FindDevice for this kind; try next probe.
                continue;
            }
            finally
            {
                foreach (var device in devices)
                {
                    device.Dispose();
                }
            }

            return;
        }
    }
}
