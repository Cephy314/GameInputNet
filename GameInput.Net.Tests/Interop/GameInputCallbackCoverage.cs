using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using GameInputDotNet.Interop.Delegates;
using GameInputDotNet.Interop.Enums;
using GameInputDotNet.Interop.Interfaces;
using GameInputDotNet.Tests.Infrastructure;
using Xunit;
using HeaderManifest = GameInputDotNet.Tests.Infrastructure.GameInputHeaderManifest;

namespace GameInputDotNet.Tests.Interop;

public static class GameInputCallbackCoverage
{
    private static readonly HeaderManifest Manifest = HeaderManifest.Load();

    private static readonly IReadOnlyDictionary<string, Type> DelegateTypesByName =
        typeof(GameInputDeviceCallback).Assembly
            .GetTypes()
            .Where(IsInteropDelegate)
            .ToDictionary(type => type.Name, StringComparer.Ordinal);

    [Fact]
    public static void AllHeaderCallbacksHaveManagedDelegates()
    {
        var expectedNames = Manifest.CallbackTypedefs
            .Select(callback => callback.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        var actualNames = DelegateTypesByName.Keys
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expectedNames, actualNames);
    }

    [Theory]
    [MemberData(nameof(GetCallbackDelegates))]
    public static void DelegateMetadataMatchesHeader(Type delegateType, GameInputCallback callbackInfo)
    {
        var unmanagedAttribute = delegateType.GetCustomAttribute<UnmanagedFunctionPointerAttribute>();
        Assert.NotNull(unmanagedAttribute);
        Assert.Equal(CallingConvention.StdCall, unmanagedAttribute!.CallingConvention);

        Assert.Equal("CALLBACK", callbackInfo.CallingConventionToken);

        var invokeMethod = delegateType.GetMethod("Invoke") ?? throw new InvalidOperationException(
            $"{delegateType.FullName} does not expose an Invoke method.");

        var parameters = invokeMethod.GetParameters();
        var expectedParameters = CallbackParameterParser.Parse(callbackInfo.ParameterSignature);
        Assert.Equal(expectedParameters.Count, parameters.Length);

        for (var index = 0; index < parameters.Length; index++)
        {
            var expected = expectedParameters[index];
            var actual = parameters[index];

            Assert.Equal(expected.ManagedType, actual.ParameterType);

            if (expected.RequiresMarshalAsInterface)
            {
                var marshalAs = actual.GetCustomAttribute<MarshalAsAttribute>();
                Assert.NotNull(marshalAs);
                Assert.Equal(UnmanagedType.Interface, marshalAs!.Value);
            }
            else
            {
                Assert.Null(actual.GetCustomAttribute<MarshalAsAttribute>());
            }
        }
    }

    public static IEnumerable<object[]> GetCallbackDelegates()
    {
        foreach (var callback in Manifest.CallbackTypedefs)
        {
            var delegateType = DelegateTypesByName[callback.Name];
            yield return new object[] { delegateType, callback };
        }
    }

    private static bool IsInteropDelegate(Type type) =>
        type.IsClass &&
        type.IsSealed &&
        type.Namespace == typeof(GameInputDeviceCallback).Namespace &&
        type.BaseType == typeof(MulticastDelegate);

    private static unsafe class CallbackParameterParser
    {
        private static readonly IReadOnlyDictionary<string, Func<ParameterExpectation>> ParameterFactories =
            new Dictionary<string, Func<ParameterExpectation>>(StringComparer.Ordinal)
            {
                ["GameInputCallbackToken"] = () => new ParameterExpectation(typeof(ulong)),
                ["void*"] = () => new ParameterExpectation(typeof(void*)),
                ["IGameInputReading*"] = () => new ParameterExpectation(typeof(IGameInputReading), RequiresMarshalAsInterface: true),
                ["IGameInputDevice*"] = () => new ParameterExpectation(typeof(IGameInputDevice), RequiresMarshalAsInterface: true),
                ["uint64_t"] = () => new ParameterExpectation(typeof(ulong)),
                ["GameInputDeviceStatus"] = () => new ParameterExpectation(typeof(GameInputDeviceStatus)),
                ["GameInputSystemButtons"] = () => new ParameterExpectation(typeof(GameInputSystemButtons)),
                ["uint32_t"] = () => new ParameterExpectation(typeof(uint))
            };

        public static IReadOnlyList<ParameterExpectation> Parse(string parameterSignature)
        {
            if (string.IsNullOrWhiteSpace(parameterSignature))
            {
                return Array.Empty<ParameterExpectation>();
            }

            var entries = parameterSignature.Split(',')
                .Select(entry => entry.Trim())
                .Where(entry => entry.Length > 0)
                .Select(RemoveSalAnnotations)
                .Select(GetNativeTypePart)
                .Select(nativeType =>
                {
                    if (!ParameterFactories.TryGetValue(nativeType, out var expectationFactory))
                    {
                        throw new NotSupportedException($"No managed mapping registered for native callback type '{nativeType}'.");
                    }

                    return expectationFactory();
                })
                .ToArray();

            return entries;
        }

        private static string RemoveSalAnnotations(string parameter) =>
            SalTokenRegex.Replace(parameter, string.Empty).Trim();

        private static string GetNativeTypePart(string parameter)
        {
            var parts = parameter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                throw new FormatException($"Unable to parse parameter declaration '{parameter}'.");
            }

            return parts[^2] switch
            {
                "*" => $"{parts[^3]}*",
                _ when parts[^2].EndsWith("*", StringComparison.Ordinal) => parts[^2],
                _ => parts[^2]
            };
        }

        private static readonly System.Text.RegularExpressions.Regex SalTokenRegex =
            new(@"\b_[A-Za-z0-9]+(?:\([^)]*\))?_?\b", System.Text.RegularExpressions.RegexOptions.Compiled);
    }

    private sealed record ParameterExpectation(Type ManagedType, bool RequiresMarshalAsInterface = false);
}
