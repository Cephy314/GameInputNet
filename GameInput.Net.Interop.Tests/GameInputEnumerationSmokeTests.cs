using System.Runtime.Versioning;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Tests.Infrastructure;

namespace GameInputDotNet.Interop.Tests;

public sealed class GameInputEnumerationSmokeTests
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