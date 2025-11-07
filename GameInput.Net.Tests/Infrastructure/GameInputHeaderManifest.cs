using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GameInputDotNet.Tests.Infrastructure;

internal sealed class GameInputHeaderManifest
{
    private GameInputHeaderManifest(
        IReadOnlyList<GameInputFunction> functions,
        IReadOnlyList<GameInputCallback> callbacks,
        IReadOnlyList<GameInputInterface> interfaces,
        string headerPath)
    {
        ExportedFunctions = functions;
        CallbackTypedefs = callbacks;
        Interfaces = interfaces;
        HeaderPath = headerPath;
    }

    public IReadOnlyList<GameInputFunction> ExportedFunctions { get; }

    public IReadOnlyList<GameInputCallback> CallbackTypedefs { get; }

    public IReadOnlyList<GameInputInterface> Interfaces { get; }

    public string HeaderPath { get; }

    public static GameInputHeaderManifest Load()
    {
        var headerPath = ResolveHeaderPath();
        if (!File.Exists(headerPath))
        {
            throw new FileNotFoundException($"GameInput header not found at '{headerPath}'.", headerPath);
        }

        var headerText = File.ReadAllText(headerPath);
        var functions = ParseFunctions(headerText);
        var callbacks = ParseCallbacks(headerText);
        var interfaces = ParseInterfaces(headerText);

        return new GameInputHeaderManifest(functions, callbacks, interfaces, headerPath);
    }

    public GameInputFunction FindFunction(string name)
    {
        return ExportedFunctions.FirstOrDefault(function =>
                   string.Equals(function.Name, name, StringComparison.Ordinal))
               ?? throw new InvalidOperationException($"GameInput.h does not declare a function named '{name}'.");
    }

    public GameInputCallback FindCallback(string name)
    {
        return CallbackTypedefs.FirstOrDefault(callback =>
                   string.Equals(callback.Name, name, StringComparison.Ordinal))
               ?? throw new InvalidOperationException($"GameInput.h does not declare a callback named '{name}'.");
    }

    public GameInputInterface FindInterface(string name)
    {
        return Interfaces.FirstOrDefault(@interface =>
                   string.Equals(@interface.Name, name, StringComparison.Ordinal))
               ?? throw new InvalidOperationException($"GameInput.h does not declare an interface named '{name}'.");
    }

    private static string ResolveHeaderPath()
    {
        // Test binaries live under GameInput.Net.Tests/bin/<Configuration>/<TargetFramework>.
        var outputDirectory = AppContext.BaseDirectory;
        var projectRoot = Path.GetFullPath(Path.Combine(outputDirectory, "..", "..", ".."));
        var solutionRoot = Directory.GetParent(projectRoot)?.FullName
                           ?? throw new DirectoryNotFoundException($"Could not locate solution root from '{projectRoot}'.");

        return Path.Combine(solutionRoot, "GameInput.Net", "GameInput.h");
    }

    private static IReadOnlyList<GameInputFunction> ParseFunctions(string headerText)
    {
        var matches = FunctionRegex.Matches(headerText);
        var functions = new List<GameInputFunction>(matches.Count);

        foreach (Match match in matches)
        {
            var name = match.Groups["name"].Value;
            var parameterBlock = match.Groups["params"].Value;
            var normalizedParameters = NormalizeWhitespace(parameterBlock);
            functions.Add(new GameInputFunction(name, normalizedParameters));
        }

        return functions;
    }

    private static IReadOnlyList<GameInputCallback> ParseCallbacks(string headerText)
    {
        var matches = CallbackRegex.Matches(headerText);
        var callbacks = new List<GameInputCallback>(matches.Count);

        foreach (Match match in matches)
        {
            var name = match.Groups["name"].Value;
            var callConv = match.Groups["callconv"].Value;
            var parameterBlock = match.Groups["params"].Value;
            var normalizedParameters = NormalizeWhitespace(parameterBlock);
            callbacks.Add(new GameInputCallback(name, callConv, normalizedParameters));
        }

        return callbacks;
    }

    private static IReadOnlyList<GameInputInterface> ParseInterfaces(string headerText)
    {
        var matches = InterfaceRegex.Matches(headerText);
        var interfaces = new List<GameInputInterface>(matches.Count);

        foreach (Match match in matches)
        {
            var name = match.Groups["name"].Value;
            var guid = match.Groups["guid"].Value;
            var body = match.Groups["body"].Value;
            var methods = ParseInterfaceMethods(body);
            interfaces.Add(new GameInputInterface(name, guid, methods));
        }

        return interfaces;
    }

    private static IReadOnlyList<GameInputInterfaceMethod> ParseInterfaceMethods(string body)
    {
        var matches = InterfaceMethodRegex.Matches(body);
        var methods = new List<GameInputInterfaceMethod>(matches.Count);

        foreach (Match match in matches)
        {
            string name;
            string returnType;
            string parameters;

            if (match.Groups["return"].Success)
            {
                returnType = NormalizeWhitespace(match.Groups["return"].Value);
                name = NormalizeWhitespace(match.Groups["name"].Value);
                parameters = match.Groups["params"].Value;
            }
            else
            {
                name = NormalizeWhitespace(match.Groups["nameOnly"].Value);
                returnType = "HRESULT";
                parameters = match.Groups["paramsOnly"].Value;
            }

            var parsedParameters = ParseMethodParameters(parameters);
            methods.Add(new GameInputInterfaceMethod(name, returnType, parsedParameters));
        }

        return methods;
    }

    private static IReadOnlyList<GameInputMethodParameter> ParseMethodParameters(string parameterBlock)
    {
        var parameters = new List<GameInputMethodParameter>();

        if (string.IsNullOrWhiteSpace(parameterBlock))
        {
            return parameters;
        }

        var segments = parameterBlock.Split(',')
            .Select(segment => segment.Trim())
            .Where(segment => segment.Length > 0);

        foreach (var segment in segments)
        {
            var salTokens = SalTokenRegex.Matches(segment)
                .Select(match => match.Value)
                .ToArray();

            var withoutSal = SalTokenRegex.Replace(segment, string.Empty).Trim();
            withoutSal = withoutSal.TrimEnd(')').Trim();
            if (withoutSal.Length == 0)
            {
                continue;
            }

            var parts = withoutSal.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;

            var name = parts[^1];
            var typeParts = parts.Take(parts.Length - 1);
            var nativeType = string.Join(" ", typeParts).Trim();

            // Handle cases where the pointer token is attached to the name (e.g. "HANDLE* waitHandle").
            if (nativeType.Length == 0 && name.Contains('*'))
            {
                var pointerIndex = name.IndexOf('*');
                nativeType = name.Substring(0, pointerIndex + 1);
                name = name.Substring(pointerIndex + 1);
            }

            parameters.Add(new GameInputMethodParameter(salTokens, nativeType, name));
        }

        return parameters;
    }

    private static string NormalizeWhitespace(string value) =>
        WhitespaceRegex.Replace(value, " ").Trim();

    private static readonly Regex FunctionRegex = new(
        @"(?ms)^\s*(?:inline\s+)?(?:HRESULT|STDAPI)\s+(?<name>GameInput\w+)\s*\((?<params>[^)]*)\)",
        RegexOptions.Compiled);

    private static readonly Regex CallbackRegex = new(
        @"typedef\s+\w+(?:\s+\w+)*\s*\(\s*(?<callconv>[A-Z_][A-Z0-9_]*)\s*\*\s*(?<name>GameInput\w+)\s*\)\s*\((?<params>[^;]*)\);",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private static readonly Regex InterfaceRegex = new(
        @"(?ms)DECLARE_INTERFACE_IID_\(\s*(?<name>\w+)\s*,\s*IUnknown\s*,\s*""(?<guid>[^""]+)""\s*\)\s*\{\s*(?<body>.*?)\s*\};",
        RegexOptions.Compiled);

    private static readonly Regex InterfaceMethodRegex = new(
        @"(?ms)IFACEMETHOD_\(\s*(?<return>[^,]+),\s*(?<name>[^\)]+)\)\s*\((?<params>[^;]*)\)\s*PURE;|IFACEMETHOD\(\s*(?<nameOnly>[^\)]+)\)\s*\((?<paramsOnly>[^;]*)\)\s*PURE;",
        RegexOptions.Compiled);

    private static readonly Regex SalTokenRegex = new(@"\b_[A-Za-z0-9]+(?:\([^)]*\))?_?\b", RegexOptions.Compiled);

    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
}

public sealed record GameInputFunction(string Name, string ParameterSignature);

public sealed record GameInputCallback(string Name, string CallingConventionToken, string ParameterSignature);

public sealed record GameInputInterface(string Name, string Guid, IReadOnlyList<GameInputInterfaceMethod> Methods);

public sealed record GameInputInterfaceMethod(string Name, string ReturnType, IReadOnlyList<GameInputMethodParameter> Parameters);

public sealed record GameInputMethodParameter(IReadOnlyList<string> SalTokens, string NativeType, string Name);
