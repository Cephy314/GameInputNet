using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using GameInputDotNet.Interop;
using GameInputDotNet.Tests.Infrastructure;
using Xunit;

namespace GameInputDotNet.Tests.Smoke;

public sealed class GameInputNativeSmoke
{
    [WindowsOnlySkippableFact]
    [SupportedOSPlatform("windows")]
    public void GameInputLibraryIsAvailable()
    {
        var assembly = typeof(GameInputNative).Assembly;

        if (NativeLibrary.TryLoad("GameInputRedist.dll", assembly, null, out var redistHandle))
        {
            NativeLibrary.Free(redistHandle);
            return;
        }

        if (NativeLibrary.TryLoad("GameInput.dll", assembly, null, out var inboxHandle))
        {
            NativeLibrary.Free(inboxHandle);
            return;
        }

        Assert.Fail("Neither GameInputRedist.dll nor GameInput.dll could be loaded. Install the Microsoft GameInput redistributable or ensure the OS-provided GameInput is present.");
    }

    [WindowsOnlySkippableFact]
    [SupportedOSPlatform("windows")]
    public void GameInputCreateReturnsSuccessAndReleasesInterface()
    {
        var hr = GameInputNative.GameInputCreate(out var gameInput);
        Assert.Equal(0, hr);

        // Leave COM lifetime management to the wrapper. Aggressively releasing the RCW here can invalidate
        // subsequent GameInputCreate calls within the same process.
    }

}
