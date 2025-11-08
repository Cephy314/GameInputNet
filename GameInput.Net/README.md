# About
GameInput.Net is a [**.NET**](https://dotnet.microsoft.com/en-us/) wrapper around Microsoft's [**GameInput**](https://learn.microsoft.com/en-us/gaming/gdk/docs/features/common/input/overviews/input-overview) library, a modern input capture system introduced woth the Microsoft [**Game Development Kit**](https://learn.microsoft.com/en-us/gaming/gdk/).

# Prerequisites
You must have Windows 10 or 11 with the GameInput library installed.  It is recommended you distribute the GameInput Redistributable with your software to ensure the user has the most recent version.  GameInput.Net will look for the re-distributable before the boxed version.

This is included in the [**Microsoft.GameInput**](https://www.nuget.org/packages/Microsoft.GameInput) package, though not automatically installed.

# How to Use
Download the package in your favorite IDE such as [**Jetbrains Rider**](https://www.jetbrains.com/rider/) or [**Microsoft Visual Studio**](https://visualstudio.microsoft.com/) using the NuGet package manager.  You may also install via commandline dotnet tools.  
```
dotnet add package GameInputNet
```

Compatible with [**.NET**](https://dotnet.microsoft.com/en-us/) 6.0+ you can instantiate a GameInput object and access all of the native [**GameInput**](https://learn.microsoft.com/en-us/gaming/gdk/docs/features/common/input/overviews/input-overview) through the managed wrapper.

```C#
using GameInputDotNet;
using GameInputDotNet.Interop.Enums;

using var _gameInput = GameInput.Create();

// Set focus allow capture of inputs regardless of program focus.
_gameInput.SetFocusPolicy(GameInputFocusPolicy.EnableBackgroundInput);

while (true)
{
    // Slow down the loop so we don't get spammed.
    Thread.Sleep(100);
    GameInputReading? reading = null;

    try
    {
        // Get the current frame's "Keyboard" inputs.
        reading = _gameInput.GetCurrentReading(GameInputKind.Keyboard);
    }
    catch (GameInputException)
    {
        // Handle exceptions and unprepared reading from first frame.
        continue;
    }

    if (reading is null) continue;

    using (reading)
    {
        // Get the current state. 
        var state = reading.GetKeyboardState();

        // Nothing pressed we can jump to the next loop.
        if (state.Keys.Count == 0)
            continue;

        // Report the scan code of each key currently pressed.
        Console.Write("Keys Pressed: ");
        foreach (var key in state.Keys) Console.Write(key.ScanCode + " ");
        Console.WriteLine();

        // We can also use this to end our program when the user hits Escape.
        if (state.Keys.Any(key => key.ScanCode == 1))
        {
            Console.WriteLine("Got ESCAPE key, ending program.");
            break;
        }
    }
}
```

# Documentation & Feedback
If you have issues or recommendations please submit a ticket through the [**GameInput.Net GitHub**](https://github.com/Cephy314/GameInputNet). The repo also contains additional documentation and examples.