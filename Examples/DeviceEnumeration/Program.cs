// See https://aka.ms/new-console-template for more information

using System.Diagnostics.CodeAnalysis;
using GameInputDotNet;
using GameInputDotNet.Interop.Enums;
using UsbVendorsLibrary;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal class Program
{
    private const string ThickHr = "=========================================";
    private const string ThinHr = "-----------------------------------------";

    // Config For Test
    private static readonly GameInputKind inputKind = GameInputKind.Keyboard;

    private static void Main(string[] args)
    {
        // Initialize GameInput Wrapper Object
        using var gameInput = GameInput.Create();
        var hr = "=========================================";

        var keyboardContainers = new Dictionary<Guid, List<GameInputDevice>>();

        // Enumerate through devices to group them by Container GUID
        foreach (var device in gameInput.EnumerateDevices(inputKind))
        {
            var info = device.GetDeviceInfo();
            if (!keyboardContainers.ContainsKey(info.ContainerId))
            {
                keyboardContainers.Add(info.ContainerId, new List<GameInputDevice>());
            }

            keyboardContainers[info.ContainerId].Add(device);
        }

        PrintSection(keyboardContainers, inputKind);
        Console.WriteLine();
    }

    private static void PrintSection(Dictionary<Guid, List<GameInputDevice>> devices, GameInputKind kind)
    {
        Console.WriteLine($"{ThickHr}\n" + $" {kind.ToString().ToUpper()}\n" + $"{ThickHr}");

        foreach (var containerId in devices.Keys)
        {
            Console.Write($"{ThickHr}\n Container: {containerId}\n{ThickHr}\n");

            foreach (var device in devices[containerId])
            {
                var info = device.GetDeviceInfo();

                var displayName = "";
                if (UsbIds.TryGetVendorName(info.VendorId, out var vendorName))
                {
                    displayName = $"{vendorName} ";
                }

                if (UsbIds.TryGetProductName(info.VendorId, info.ProductId, out var productName))
                {
                    displayName += $"{productName}";
                }
                else
                {
                    displayName += info.GetDisplayName();
                }

                Console.Write($"ID: {info.DeviceId}\n" + $"DisplayName: {displayName}\n");
                Console.Write($"{ThinHr}\n");
            }

            Console.WriteLine(ThickHr + "\n");
        }
    }
}