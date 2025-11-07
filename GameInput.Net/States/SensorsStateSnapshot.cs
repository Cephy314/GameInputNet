using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Structs;

namespace GameInputDotNet.States;

public sealed class SensorsStateSnapshot
{
    internal SensorsStateSnapshot(GameInputSensorsState state)
    {
        AccelerationInGx = state.AccelerationInGx;
        AccelerationInGy = state.AccelerationInGy;
        AccelerationInGz = state.AccelerationInGz;
        AngularVelocityInRadPerSecX = state.AngularVelocityInRadPerSecX;
        AngularVelocityInRadPerSecY = state.AngularVelocityInRadPerSecY;
        AngularVelocityInRadPerSecZ = state.AngularVelocityInRadPerSecZ;
        HeadingInDegreesFromMagneticNorth = state.HeadingInDegreesFromMagneticNorth;
        HeadingAccuracy = state.HeadingAccuracy;
        OrientationW = state.OrientationW;
        OrientationX = state.OrientationX;
        OrientationY = state.OrientationY;
        OrientationZ = state.OrientationZ;
    }

    public float AccelerationInGx { get; }
    public float AccelerationInGy { get; }
    public float AccelerationInGz { get; }

    public float AngularVelocityInRadPerSecX { get; }
    public float AngularVelocityInRadPerSecY { get; }
    public float AngularVelocityInRadPerSecZ { get; }

    public float HeadingInDegreesFromMagneticNorth { get; }
    public GameInputSensorAccuracy HeadingAccuracy { get; }

    public float OrientationW { get; }
    public float OrientationX { get; }
    public float OrientationY { get; }
    public float OrientationZ { get; }
}
