using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using GameInputDotNet;
using Xunit;

namespace GameInputDotNet.Interop.Tests;

public sealed class InteropInterfaceSignatureTests
{
    private static readonly Assembly InteropAssembly = typeof(GameInputFactory).Assembly;

    private static readonly Type[] InterfaceTypes = InteropAssembly
        .GetTypes()
        .Where(type =>
            type.IsInterface &&
            string.Equals(type.Namespace, "GameInputDotNet.Interop.Interfaces", StringComparison.Ordinal))
        .ToArray();

    [Fact]
    public void AllInterfacesAreMarkedForComInterop()
    {
        foreach (var interfaceType in InterfaceTypes)
        {
            Assert.NotNull(interfaceType.GetCustomAttribute<ComImportAttribute>());
            var guidAttribute = interfaceType.GetCustomAttribute<GuidAttribute>();
            Assert.NotNull(guidAttribute);
            Assert.True(Guid.TryParse(guidAttribute!.Value, out _), $"{interfaceType.FullName} GUID is invalid.");

            var interfaceTypeAttribute = interfaceType.GetCustomAttribute<InterfaceTypeAttribute>();
            Assert.NotNull(interfaceTypeAttribute);
            Assert.Equal(ComInterfaceType.InterfaceIsIUnknown, interfaceTypeAttribute!.Value);
        }
    }

    [Fact]
    public void AllInterfaceMethodsPreserveSignatures()
    {
        foreach (var method in InterfaceTypes.SelectMany(type => type.GetMethods()))
        {
            Assert.NotNull(method.GetCustomAttribute<PreserveSigAttribute>());
        }
    }

    [Fact]
    public void BooleanReturnsAreExplicitlyMarshalled()
    {
        foreach (var method in InterfaceTypes.SelectMany(type => type.GetMethods()))
        {
            if (method.ReturnType != typeof(bool)) continue;

            var marshalAs = method.ReturnParameter.GetCustomAttribute<MarshalAsAttribute>();
            Assert.NotNull(marshalAs);
            Assert.Equal(UnmanagedType.Bool, marshalAs!.Value);
        }
    }

    [Fact]
    public void InterfaceParametersHaveExpectedMarshalAttributes()
    {
        foreach (var method in InterfaceTypes.SelectMany(type => type.GetMethods()))
        {
            foreach (var parameter in method.GetParameters())
            {
                var expected = ParameterMarshallingRules.Resolve(parameter);
                if (expected is null) continue;

                expected.Validate(parameter);
            }
        }
    }

    private static class ParameterMarshallingRules
    {
        public static ParameterExpectation? Resolve(ParameterInfo parameter)
        {
            if (parameter.ParameterType.IsInterface)
            {
                if (parameter.GetCustomAttribute<MarshalAsAttribute>() is not { Value: UnmanagedType.Interface })
                {
                    return ParameterExpectation.RequiresMarshalAs(UnmanagedType.Interface);
                }

                return null;
            }

            if (parameter.ParameterType.IsPointer)
            {
                // Pointer-typed parameters should not carry MarshalAs (blittable).
                return ParameterExpectation.NoMarshalAs();
            }

            if (parameter.ParameterType == typeof(bool))
            {
                if (parameter.GetCustomAttribute<MarshalAsAttribute>() is not { Value: UnmanagedType.Bool })
                {
                    return ParameterExpectation.RequiresMarshalAs(UnmanagedType.Bool);
                }

                return null;
            }

            if (parameter.ParameterType == typeof(string))
            {
                if (parameter.GetCustomAttribute<MarshalAsAttribute>() is not { Value: UnmanagedType.LPWStr })
                {
                    return ParameterExpectation.RequiresMarshalAs(UnmanagedType.LPWStr);
                }

                return null;
            }

            return null;
        }
    }

    private sealed record ParameterExpectation(bool MarshalAsExpected, UnmanagedType? ExpectedUnmanagedType = null)
    {
        public static ParameterExpectation RequiresMarshalAs(UnmanagedType unmanagedType) =>
            new(true, unmanagedType);

        public static ParameterExpectation NoMarshalAs() =>
            new(false, null);

        public void Validate(ParameterInfo parameter)
        {
            var marshalAs = parameter.GetCustomAttribute<MarshalAsAttribute>();
            if (!MarshalAsExpected)
            {
                Assert.Null(marshalAs);
                return;
            }

            Assert.NotNull(marshalAs);
            Assert.Equal(ExpectedUnmanagedType, marshalAs!.Value);
        }
    }
}
