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
    public sealed class GameInputReadingNotFoundException : GameInputException
    {
        public GameInputReadingNotFoundException(string message, int hresult)
            : base(message, hresult)
        {
        }
    }

    public sealed class GameInputDeviceNotConnectedException : GameInputException
    {
        public GameInputDeviceNotConnectedException(string message, int hresult)
            : base(message, hresult)
        {
        }
    }

    public sealed class GameInputDeviceNotFoundException : GameInputException
    {
        public GameInputDeviceNotFoundException(string message, int hresult)
            : base(message, hresult)
        {
        }
    }

    public sealed class GameInputReferenceReadingTooOldException : GameInputException
    {
        public GameInputReferenceReadingTooOldException(string message, int hresult)
            : base(message, hresult)
        {
        }
    }

    public sealed class GameInputFeedbackNotSupportedException : GameInputException
    {
        public GameInputFeedbackNotSupportedException(string message, int hresult)
            : base(message, hresult)
        {
        }
    }

    public sealed class GameInputObjectNoLongerExistsException : GameInputException
    {
        public GameInputObjectNoLongerExistsException(string message, int hresult)
            : base(message, hresult)
        {
        }
    }

    public sealed class GameInputCallbackNotFoundException : GameInputException
    {
        public GameInputCallbackNotFoundException(string message, int hresult)
            : base(message, hresult)
        {
        }
    }

    public sealed class GameInputHapticInfoNotFoundException : GameInputException
    {
        public GameInputHapticInfoNotFoundException(string message, int hresult)
            : base(message, hresult)
        {
        }
    }

    public sealed class GameInputAggregateOperationNotSupportedException : GameInputException
    {
        public GameInputAggregateOperationNotSupportedException(string message, int hresult)
            : base(message, hresult)
        {
        }
    }

    public sealed class GameInputInputKindNotPresentException : GameInputException
    {
        public GameInputInputKindNotPresentException(string message, int hresult)
            : base(message, hresult)
        {
        }
    }
}
