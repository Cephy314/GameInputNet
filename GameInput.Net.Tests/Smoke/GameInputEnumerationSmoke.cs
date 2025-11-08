using System;
using System.Runtime.Versioning;
using GameInputDotNet;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Tests.Infrastructure;
using Xunit;

namespace GameInputDotNet.Tests.Smoke;

public sealed class GameInputEnumerationSmoke
{
    [WindowsOnlySkippableFact]
    [SupportedOSPlatform("windows")]
    public void EnumerateDevices_CompletesAndDisposes()
    {
        using var gameInput = GameInputFactory.Create();
        var controllers = gameInput.EnumerateDevices(GameInputKind.Controller);

        var enumeratedAny = false;
        foreach (var controller in controllers)
        {
            enumeratedAny = true;
            controller.Dispose();
        }

        if (!enumeratedAny)
        {
            Console.WriteLine("Skipping enumeration smoke: no controller-class devices present.");
            Skip.If(true, "Device enumeration yielded no controller-class devices. Connect hardware or verify the redistributable is exposing controllers.");
        }
    }
}
