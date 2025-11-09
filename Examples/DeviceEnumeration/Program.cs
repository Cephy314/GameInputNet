// See https://aka.ms/new-console-template for more information

using GameInputDotNet;
using GameInputDotNet.Interop.Enums;

internal class Program
{
    private static void Main(string[] args)
    {
        // Initialize GameInput Wrapper Object
        using var gameInput = GameInput.Create();

        while (true)
        {
            Thread.Sleep(1000);
            try
            {
                var index = 1;
                foreach (var input in
                         gameInput.EnumerateDevices(GameInputKind.Controller))
                {
                    if (input == null)
                    {
                        Console.WriteLine("No keyboard input");
                        Thread.Sleep(1000);
                        continue;
                    }

                    var info = input.GetDeviceInfo();

                    Console.WriteLine(
                        $"{index}:{info.ProductId}/{info.GetPnpPath()}: {info.SupportedInput}");
                    index++;
                }
            }
            catch (Exception ex)
            {
                var errorCode = (GameInputErrorCode)ex.HResult;
                Console.WriteLine(ex.Message);
            }
        }
    }
}