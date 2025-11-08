using System.Runtime.InteropServices;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Primitives;

namespace GameInputDotNet.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct GameInputForceFeedbackMotorInfo
{
    public GameInputFeedbackAxes SupportedAxes;

    public GameInputBoolean IsConstantEffectSupported;

    public GameInputBoolean IsRampEffectSupported;

    public GameInputBoolean IsSineWaveEffectSupported;

    public GameInputBoolean IsSquareWaveEffectSupported;

    public GameInputBoolean IsTriangleWaveEffectSupported;

    public GameInputBoolean IsSawtoothUpWaveEffectSupported;

    public GameInputBoolean IsSawtoothDownWaveEffectSupported;

    public GameInputBoolean IsSpringEffectSupported;

    public GameInputBoolean IsFrictionEffectSupported;

    public GameInputBoolean IsDamperEffectSupported;

    public GameInputBoolean IsInertiaEffectSupported;
}
