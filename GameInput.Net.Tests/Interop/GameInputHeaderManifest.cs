using System;
using System.Linq;
using GameInputDotNet.Tests.Infrastructure;
using Xunit;
using HeaderManifest = GameInputDotNet.Tests.Infrastructure.GameInputHeaderManifest;

namespace GameInputDotNet.Tests.Interop;

public sealed class GameInputHeaderManifestChecks
{
    private static readonly HeaderManifest Manifest = HeaderManifest.Load();

    [Fact]
    public void CallbackTypedefsMatchHeader()
    {
        var expectedNames = new[]
        {
            "GameInputReadingCallback",
            "GameInputDeviceCallback",
            "GameInputSystemButtonCallback",
            "GameInputKeyboardLayoutCallback"
        };

        var actualNames = Manifest.CallbackTypedefs
            .Select(callback => callback.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expectedNames.OrderBy(name => name, StringComparer.Ordinal).ToArray(), actualNames);
    }

    [Fact]
    public void CallbackCallingConventionTokensAreCaptured()
    {
        foreach (var callback in Manifest.CallbackTypedefs)
        {
            Assert.Equal("CALLBACK", callback.CallingConventionToken);
            Assert.False(string.IsNullOrWhiteSpace(callback.ParameterSignature));
        }
    }
}
