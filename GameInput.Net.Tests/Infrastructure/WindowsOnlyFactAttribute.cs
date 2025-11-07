using System;
using Xunit;

namespace GameInputDotNet.Tests.Infrastructure;

internal sealed class WindowsOnlyFactAttribute : FactAttribute
{
    public WindowsOnlyFactAttribute()
    {
        if (!OperatingSystem.IsWindows())
        {
            Skip = "Requires Windows to load GameInput native dependencies.";
        }
    }
}
