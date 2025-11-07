using System;
using System.Runtime.Versioning;
using GameInputDotNet;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Tests.Infrastructure;

namespace GameInputDotNet.Tests.Smoke;

public sealed class GameInputEnumerationSmoke
{
    [WindowsOnlyFact]
    [SupportedOSPlatform("windows")]
    public void EnumerateDevices_CompletesAndDisposes()
    {
        using var gameInput = GameInputFactory.Create();
        var controllers = gameInput.EnumerateDevices(GameInputKind.Controller);

        Console.WriteLine($"Controllers count: {controllers.Count}");
        foreach (var controller in controllers) controller.Dispose();
        Console.WriteLine($"Disposed controllers: {controllers.Count}");
    }
}
