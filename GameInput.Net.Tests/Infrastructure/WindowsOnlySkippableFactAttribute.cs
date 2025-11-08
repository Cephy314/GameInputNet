using System;
using Xunit;
using Xunit.Sdk;

namespace GameInputDotNet.Tests.Infrastructure;

internal sealed class WindowsOnlySkippableFactAttribute : SkippableFactAttribute
{
    public WindowsOnlySkippableFactAttribute()
    {
        if (!OperatingSystem.IsWindows())
        {
            Skip = "Requires Windows to load GameInput native dependencies.";
        }
    }
}
