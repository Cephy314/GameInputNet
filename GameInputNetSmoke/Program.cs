using GameInputNet;
using GameInputFocusPolicy = GameInputNet.Interop.GameInputFocusPolicy;

Console.WriteLine("Creating GameInput instance...");
using var gameInput = GameInputFactory.Create();

Console.WriteLine($"GameInput timestamp: {gameInput.GetCurrentTimestamp()}");
Console.WriteLine("Applying default focus policy...");
gameInput.SetFocusPolicy(GameInputFocusPolicy.Default);

Console.WriteLine("Done.");
