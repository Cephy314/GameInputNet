using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using GameInputDotNet.Interop;
using GameInputDotNet.Interop.Interfaces;
using GameInputDotNet.Tests.Infrastructure;
using Xunit;
using HeaderManifest = GameInputDotNet.Tests.Infrastructure.GameInputHeaderManifest;

namespace GameInputDotNet.Tests.Interop;

public sealed class GameInputNativeSignature
{
    private static readonly HeaderManifest Manifest = HeaderManifest.Load();

    private static readonly HashSet<string> IgnoredHeaderFunctions = new(StringComparer.Ordinal)
    {
        // GameInputCreate already wraps GameInputInitialize; we intentionally expose only the higher-level helper.
        "GameInputInitialize"
    };

    private static readonly IReadOnlyDictionary<string,
        Action<IReadOnlyList<ParameterInfo>, GameInputFunction>> ParameterVerifiers =
        new Dictionary<string, Action<IReadOnlyList<ParameterInfo>, GameInputFunction>>(StringComparer.Ordinal)
        {
            [nameof(GameInputNative.GameInputCreate)] = AssertGameInputCreateParameters
        };

    [Fact]
    public void DllImportsCoverHeaderFunctions()
    {
        var expectedNames = Manifest.ExportedFunctions
            .Select(function => function.Name)
            .Where(name => !IgnoredHeaderFunctions.Contains(name))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        var actualNames = GetDllImportMethods()
            .Select(method => method.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expectedNames, actualNames);
    }

    [Theory]
    [MemberData(nameof(GetDllImportMetadata))]
    public void DllImportMetadataMatchesHeader(MethodInfo method, GameInputFunction headerFunction)
    {
        var dllImport = method.GetCustomAttribute<DllImportAttribute>();
        Assert.NotNull(dllImport);

        Assert.Equal("GameInput.dll", dllImport!.Value);
        Assert.Equal(CallingConvention.StdCall, dllImport.CallingConvention);
        Assert.True(dllImport.ExactSpelling);

        Assert.True(method.MethodImplementationFlags.HasFlag(MethodImplAttributes.PreserveSig));
        Assert.True(method.IsStatic);
        Assert.False(method.IsGenericMethodDefinition);
        Assert.Equal(typeof(int), method.ReturnType);

        var parameterVerifier = ParameterVerifiers.GetValueOrDefault(method.Name);
        Assert.NotNull(parameterVerifier);

        parameterVerifier!(method.GetParameters(), headerFunction);
    }

    public static IEnumerable<object[]> GetDllImportMetadata()
    {
        foreach (var method in GetDllImportMethods())
        {
            if (IgnoredHeaderFunctions.Contains(method.Name))
            {
                continue;
            }

            var headerFunction = Manifest.FindFunction(method.Name);
            yield return new object[] { method, headerFunction };
        }
    }

    private static IEnumerable<MethodInfo> GetDllImportMethods() =>
        typeof(GameInputNative)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => method.GetCustomAttribute<DllImportAttribute>() is not null);

    private static void AssertGameInputCreateParameters(
        IReadOnlyList<ParameterInfo> parameters,
        GameInputFunction headerFunction)
    {
        Assert.Single(parameters);
        Assert.Equal("_COM_Outptr_ IGameInput** gameInput", headerFunction.ParameterSignature);

        var parameter = parameters[0];
        Assert.True(parameter.IsOut);
        Assert.True(parameter.ParameterType.IsByRef);
        Assert.Equal(typeof(IGameInput), parameter.ParameterType.GetElementType());
        Assert.Equal(typeof(IGameInput).MakeByRefType(), parameter.ParameterType);
        Assert.Null(parameter.GetCustomAttribute<MarshalAsAttribute>());
    }
}
