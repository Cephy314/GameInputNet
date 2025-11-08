| Previous                  |                   | Next                      |
| ------------------------- | ----------------- | ------------------------- |
| [Overview](1-Overview.md) | [Home](README.md) | [Readings](3-Readings.md) |

# GameInput.Net Fundamentals
*For detailed information please visit the [official documentation](https://learn.microsoft.com/en-us/gaming/gdk/docs/features/common/input/overviews/input-fundamentals) for GameInput*

GameInput is built around the idea of an input stream; input from all devices connected to the system are added to a contiguous stream of events, in the order the input occured.  When multiple devices generate input simultaneously, this results in a natural interleaving of the input events. 

Each element in the input stream is represented by a **reading**.  Applications begin by getting the most current reading from the input stream.  From there the application can either continue to periodically poll the stream for the most current reading, or it can walk forward and backwards through the input stream, examining adjacent readings.  In either case, the application typically compares the differences between readings of interest, and then takes the appropriate actions.

For example, a first-person shooter game might only need to know whether a button is pressed at the start of each frame to determine whether the player's weapon should fire.  In that case, only the most current reading matters (and any intermediate readings can be ignored).  On the other hand, a competitive fighting game might want to examine every input state change that occured between the last frame and the one for any of the following reasons.
- Double-tapping a button might map to a different combo/move than a single tap.
- The order that the two buttons were pressed in might matter.
- The elapsed time between two button presses might matter.

When retrieving readings from the input stream, applications can apply optional filters, which limit the results to readings from specific input types (such as gamepads or keyboards), and/or from specific devices.  For more details see [Readings](3-Readings.md). 

Applications can only request historical readings from the input stream that occured within the last half second.  The most current reading from each connected device is always accessible, no matter how old it is.

# Classes
The base GameInput code provides interfaces for accessing the the functionality of the library, but to work within C# and .NET conventions we have provided classes that allow the same access and simplify memory management, lifetimes and needing to deal with complex pointers and interops. 

```mermaid

```