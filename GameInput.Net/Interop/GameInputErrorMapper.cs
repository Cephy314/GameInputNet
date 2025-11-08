using GameInputDotNet;

namespace GameInputDotNet.Interop
{
    internal static class GameInputErrorMapper
    {
        public static void ThrowIfFailed(int hresult, string context)
        {
            if (HResult.SUCCEEDED(hresult)) return;

            switch ((GameInputErrorCode)hresult)
            {
                case GameInputErrorCode.DeviceDisconnected:
                    throw new GameInputDeviceNotConnectedException(context, hresult);
                case GameInputErrorCode.DeviceNotFound:
                    throw new GameInputDeviceNotFoundException(context, hresult);
                case GameInputErrorCode.ReadingNotFound:
                    throw new GameInputReadingNotFoundException(context, hresult);
                case GameInputErrorCode.ReferenceReadingTooOld:
                    throw new GameInputReferenceReadingTooOldException(context, hresult);
                case GameInputErrorCode.FeedbackNotSupported:
                    throw new GameInputFeedbackNotSupportedException(context, hresult);
                case GameInputErrorCode.ObjectNoLongerExists:
                    throw new GameInputObjectNoLongerExistsException(context, hresult);
                case GameInputErrorCode.CallbackNotFound:
                    throw new GameInputCallbackNotFoundException(context, hresult);
                case GameInputErrorCode.HapticInfoNotFound:
                    throw new GameInputHapticInfoNotFoundException(context, hresult);
                case GameInputErrorCode.AggregateOperationNotSupported:
                    throw new GameInputAggregateOperationNotSupportedException(context, hresult);
                case GameInputErrorCode.InputKindNotPresent:
                    throw new GameInputInputKindNotPresentException(context, hresult);
                default:
                    throw new GameInputException(context, hresult);
            }
        }
    }
}

namespace GameInputDotNet
{
    public sealed class GameInputReadingNotFoundException(string message, int hresult)
        : GameInputException(message, hresult);

    public sealed class GameInputDeviceNotConnectedException(string message, int hresult)
        : GameInputException(message, hresult);

    public sealed class GameInputDeviceNotFoundException(string message, int hresult)
        : GameInputException(message, hresult);

    public sealed class GameInputReferenceReadingTooOldException(string message, int hresult)
        : GameInputException(message, hresult);

    public sealed class GameInputFeedbackNotSupportedException(string message, int hresult)
        : GameInputException(message, hresult);

    public sealed class GameInputObjectNoLongerExistsException(string message, int hresult)
        : GameInputException(message, hresult);

    public sealed class GameInputCallbackNotFoundException(string message, int hresult)
        : GameInputException(message, hresult);

    public sealed class GameInputHapticInfoNotFoundException(string message, int hresult)
        : GameInputException(message, hresult);

    public sealed class GameInputAggregateOperationNotSupportedException(string message, int hresult)
        : GameInputException(message, hresult);

    public sealed class GameInputInputKindNotPresentException(string message, int hresult)
        : GameInputException(message, hresult);
}
