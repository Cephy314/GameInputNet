namespace GameInputDotNet;

/// <summary>
///     Represents the well-known error codes returned by the native GameInput runtime.
/// </summary>
public enum GameInputErrorCode : int
{
    DeviceDisconnected = unchecked((int)0x838A0001),
    DeviceNotFound = unchecked((int)0x838A0002),
    ReadingNotFound = unchecked((int)0x838A0003),
    ReferenceReadingTooOld = unchecked((int)0x838A0004),
    FeedbackNotSupported = unchecked((int)0x838A0007),
    ObjectNoLongerExists = unchecked((int)0x838A0008),
    CallbackNotFound = unchecked((int)0x838A0009),
    HapticInfoNotFound = unchecked((int)0x838A000A),
    AggregateOperationNotSupported = unchecked((int)0x838A000B),
    InputKindNotPresent = unchecked((int)0x838A000C)
}
