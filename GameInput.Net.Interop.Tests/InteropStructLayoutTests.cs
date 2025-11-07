using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using GameInputDotNet;
using Xunit;

namespace GameInputDotNet.Interop.Tests;

public sealed class InteropStructLayoutTests
{
    private static readonly Assembly InteropAssembly = typeof(GameInputFactory).Assembly;

    private static readonly Type[] StructTypes = InteropAssembly
        .GetTypes()
        .Where(type =>
            type.IsValueType &&
            !type.IsEnum &&
            string.Equals(type.Namespace, "GameInputDotNet.Interop.Structs", StringComparison.Ordinal))
        .ToArray();

    [Fact]
    public void AllInteropStructsUseSequentialLayout()
    {
        foreach (var type in StructTypes)
        {
            var layout = type.StructLayoutAttribute;
            Assert.NotNull(layout);
            Assert.Contains(layout!.Value, new[] { LayoutKind.Sequential, LayoutKind.Explicit });
        }
    }

    [Fact]
    public void AllInteropStructsAreBlittable()
    {
        foreach (var type in StructTypes)
        {
            Assert.True(IsBlittable(type), $"{type.FullName} is not blittable.");
        }
    }

    [Fact]
    public void MarshalSizeIsStableForInteropStructs()
    {
        foreach (var type in StructTypes)
        {
            var size = Marshal.SizeOf(type);
            Assert.True(size > 0, $"{type.FullName} reported an unexpected size.");
        }
    }

    private static bool IsBlittable(Type type)
    {
        try
        {
            var array = Array.CreateInstance(type, 1);
            var handle = GCHandle.Alloc(array, GCHandleType.Pinned);
            handle.Free();
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
