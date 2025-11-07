using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using GameInputDotNet.Interop;
using GameInputDotNet.Tests.Infrastructure;
using Xunit;

namespace GameInputDotNet.Tests.Smoke;

public sealed class GameInputNativeSmoke
{
    [WindowsOnlyFact]
    [SupportedOSPlatform("windows")]
    public void GameInputLibraryIsDeployedToTestOutput()
    {
        var nativePath = ResolveNativePath();
        Assert.True(File.Exists(nativePath),
            $"GameInput.dll should exist at '{nativePath}' for smoke tests.");
    }

    [WindowsOnlyFact]
    [SupportedOSPlatform("windows")]
    public void GameInputCreateReturnsSuccessAndReleasesInterface()
    {
        var hr = GameInputNative.GameInputCreate(out var gameInput);
        Assert.Equal(0, hr);

        // Leave COM lifetime management to the wrapper. Aggressively releasing the RCW here can invalidate
        // subsequent GameInputCreate calls within the same process.
    }

    private static string ResolveNativePath()
    {
        var baseDirectory = AppContext.BaseDirectory;
        return Path.Combine(baseDirectory, "GameInput.dll");
    }
}
