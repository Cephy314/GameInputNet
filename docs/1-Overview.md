[Back to README.md](README.md)
# Overview of GameInput.Net
Microsoft has created a new library Microsoft.GameInput as part of their Game Development Kit that is usable on windows and X-Box bringing input mapping and reading to the current century.  Reliance on XInput and other old implementations has caused no end of frustrating limitations.  This project aims to provide a managed wrapper to allow developers and hobbyists to access the GameInput system which is either natively installed with Windows 10 and Windows 11 or through a Microsoft Redistributable Package freely available to anyone.  

Quoted passages below come from [Overview of GameInput on learn.microsoft.com](https://learn.microsoft.com/en-us/gaming/gdk/docs/features/common/input/overviews/input-overview)

## Availability
Via this library GameInput is only available on Windows 10 or 11 from Windows 10 19H1 or newer, it will not work for X-Box or Mobile.  If you wish to use the libraries on other platforms you must go through Microsoft and their licensing programs.

## Consistency
> GameInput exposes input from keyboards, mice, gamepads and other game controllers via a single unified input model, synchronized to a common time base.  The code for handing input from these devices is nearly identical, using many of the same function but with different filters applied.  This makes it easy to add support for additional input devices, without major changes to input code.

This library aims to extend this consistency and maintain a stable, simple and easy to use interface that matches the GameInput library with C# / DotNet standards and best practices.  The wrapper presents the user with a fully managed interface so they will not have to touch any unmanaged or unsafe code.

## Functionality 
> GameInput is a functional superset of all legacy input APIs—XInput, DirectInput, Raw Input, Human Interface Device (HID), and WinRT APIs—in addition to adding new features of its own. GameInput's functionality ranges from simple fixed-format gamepad state to detailed low-level raw device access. Input can be obtained via polling or callbacks in an event-driven way. Haptics and force feedback are fully supported, and third-party device SDKs can easily be written on top of GameInput to provide access to custom device features.

## Performance
> GameInput is built around an entirely new direct memory access (DMA) architecture for the lowest possible input latency and resource usage. Nearly all API functions are lock-free with strict performance guarantees, while still being 100% thread-safe. This makes them safe to call from time-sensitive contexts such as render threads. Advanced applications can take direct control of scheduling GameInput's internal asynchronous work queues, controlling which thread does the work and how often.

Inevitably there will be some level of performance loss creating a managed layer on top of the base library, but this project will strive to maintain the best possible speeds and expose all the tools mentioned above for the developers to tune their input experience.

# Getting Started
Microsoft recommends using the GameInput library for all new code, regardless of target platform as it provides support across all microsoft platforms including earlier versions of windows, and provides superior performance versus legacy APIs.

Fundamentals
Reading
Devices
