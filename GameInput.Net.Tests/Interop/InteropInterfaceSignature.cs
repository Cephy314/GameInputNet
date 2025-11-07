using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using GameInputDotNet;
using GameInputDotNet.Tests.Infrastructure;
using Xunit;
using HeaderManifest = GameInputDotNet.Tests.Infrastructure.GameInputHeaderManifest;

namespace GameInputDotNet.Tests.Interop;

public sealed class InteropInterfaceSignature
{
    private static readonly HeaderManifest Manifest = HeaderManifest.Load();

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
            var headerInterface = Manifest.FindInterface(interfaceType.Name);

            Assert.NotNull(interfaceType.GetCustomAttribute<ComImportAttribute>());
            var guidAttribute = interfaceType.GetCustomAttribute<GuidAttribute>();
            Assert.NotNull(guidAttribute);
            Assert.True(Guid.TryParse(guidAttribute!.Value, out _), $"{interfaceType.FullName} GUID is invalid.");

            var interfaceTypeAttribute = interfaceType.GetCustomAttribute<InterfaceTypeAttribute>();
            Assert.NotNull(interfaceTypeAttribute);
            Assert.Equal(ComInterfaceType.InterfaceIsIUnknown, interfaceTypeAttribute!.Value);

            Assert.True(string.Equals(headerInterface.Guid, guidAttribute!.Value, StringComparison.OrdinalIgnoreCase),
                $"{interfaceType.FullName} GUID mismatch. Header: {headerInterface.Guid}, Managed: {guidAttribute!.Value}");
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

    [Fact]
    public void ManagedInterfacesMatchHeaderSignatures()
    {
        foreach (var interfaceType in InterfaceTypes)
        {
            var headerInterface = Manifest.FindInterface(interfaceType.Name);
            var managedMethods = interfaceType.GetMethods();
            Assert.Equal(headerInterface.Methods.Count, managedMethods.Length);

            foreach (var headerMethod in headerInterface.Methods)
            {
                var method = managedMethods.SingleOrDefault(m => string.Equals(m.Name, headerMethod.Name, StringComparison.Ordinal));
                Assert.NotNull(method);

                var managedParameters = method!.GetParameters();
                Assert.Equal(headerMethod.Parameters.Count, managedParameters.Length);

                for (var index = 0; index < managedParameters.Length; index++)
                {
                    var headerParameter = headerMethod.Parameters[index];
                    var managedParameter = managedParameters[index];
                    ValidateParameterSignature(interfaceType, method, headerParameter, managedParameter);
                }
            }
        }
    }

    private static void ValidateParameterSignature(
        Type interfaceType,
        MethodInfo method,
        GameInputMethodParameter headerParameter,
        ParameterInfo managedParameter)
    {
        var pointerDepth = CountPointerDepth(headerParameter.NativeType);
        var hasComOut = ContainsSalToken(headerParameter.SalTokens, "_COM_Outptr");
        var hasOut = ContainsSalToken(headerParameter.SalTokens, "_Out");
        var hasInOut = ContainsSalToken(headerParameter.SalTokens, "_Inout");

        var isInterfacePointer = pointerDepth == 1 && headerParameter.NativeType.Contains("IGameInput", StringComparison.Ordinal);

        if (pointerDepth > 0 && !isInterfacePointer)
        {
            Assert.True(managedParameter.ParameterType.IsPointer || managedParameter.ParameterType.IsByRef,
                $"{DescribeParameter(interfaceType, method, managedParameter)} should marshal as a pointer or by-ref to mirror native type '{headerParameter.NativeType}'.");
        }

        if (hasComOut || (pointerDepth >= 2))
        {
            Assert.True(managedParameter.ParameterType.IsByRef,
                $"{DescribeParameter(interfaceType, method, managedParameter)} must be declared as 'out' to satisfy native definition '{headerParameter.NativeType}'.");
        }
        else if (hasOut && !managedParameter.ParameterType.IsPointer)
        {
            Assert.True(managedParameter.IsOut || managedParameter.ParameterType.IsByRef,
                $"{DescribeParameter(interfaceType, method, managedParameter)} should be declared as 'out' based on SAL annotation.");
        }

        if (!hasOut && !hasComOut && !hasInOut && pointerDepth == 0 && !managedParameter.ParameterType.IsPointer)
        {
            Assert.False(managedParameter.IsOut && !managedParameter.IsIn,
                $"{DescribeParameter(interfaceType, method, managedParameter)} should not be declared as 'out'.");
        }
    }

    private static string DescribeParameter(Type interfaceType, MethodInfo method, ParameterInfo parameter) =>
        $"{interfaceType.Name}.{method.Name}::{parameter.Name}";

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

    private static bool ContainsSalToken(IEnumerable<string> tokens, string value) =>
        tokens.Any(token => token.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0);

    private static int CountPointerDepth(string nativeType)
    {
        var depth = 0;
        foreach (var character in nativeType)
        {
            if (character == '*') depth++;
        }

        return depth;
    }
}
