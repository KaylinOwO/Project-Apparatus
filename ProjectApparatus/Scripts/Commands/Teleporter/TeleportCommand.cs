using UnityEngine;
using GameNetcodeStuff;
using Hax;
using System.Linq;

public class TeleportResult
{
    public bool Success { get; }
    public string Message { get; }

    private TeleportResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public static TeleportResult Create(bool success, string message)
    {
        return new TeleportResult(success, message ?? "Unknown error");
    }
}

[Command("/tp")]
public class TeleportCommand : ITeleporter, ICommand
{
    private Vector3? GetCoordinates(string[] args)
    {
        float x, y, z;
        bool isValidX = float.TryParse(args[0], out x);
        bool isValidY = float.TryParse(args[1], out y);
        bool isValidZ = float.TryParse(args[2], out z);

        return !isValidX || !isValidY || !isValidZ ? null : new Vector3(x, y, z);
    }

    private TeleportResult TeleportToPlayer(string[] args)
    {
        PlayerControllerB targetPlayer;
        PlayerControllerB currentPlayer = Helper.LocalPlayer;

        if (!Helper.GetPlayer(args[0]).IsNotNull(out targetPlayer) || currentPlayer == null)
        {
            return TeleportResult.Create(false, "Player not found!");
        }

        currentPlayer.TeleportPlayer(targetPlayer.transform.position);
        return TeleportResult.Create(true, "");
    }

    private TeleportResult TeleportToPosition(string[] args)
    {
        Vector3? coordinates = this.GetCoordinates(args);

        if (coordinates is null)
        {
            return TeleportResult.Create(false, "Invalid coordinates!");
        }

        Helper.LocalPlayer?.TeleportPlayer(coordinates.Value);
        return TeleportResult.Create(true, "");
    }

    private TeleportResult TeleportPlayerToPosition(PlayerControllerB player, Vector3 position)
    {
        this.PrepareToTeleport(this.TeleportPlayerToPositionLater(player, position));
        return TeleportResult.Create(true, "");
    }

    private TeleportResult TeleportPlayerToPosition(string[] args)
    {
        if (!Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB player))
        {
            return TeleportResult.Create(false, "Player not found!");
        }

        Vector3? coordinates = this.GetCoordinates(args.Skip(1).ToArray());

        return coordinates is null
            ? TeleportResult.Create(false, "Invalid coordinates!")
            : this.TeleportPlayerToPosition(player, coordinates.Value);
    }

    private TeleportResult TeleportPlayerToPlayer(string[] args)
    {
        PlayerControllerB sourcePlayer;
        PlayerControllerB targetPlayer;

        if (!Helper.GetPlayer(args[0]).IsNotNull(out sourcePlayer) || !Helper.GetPlayer(args[1]).IsNotNull(out targetPlayer))
        {
            return TeleportResult.Create(false, "Player not found!");
        }

        this.TeleportPlayerToPosition(sourcePlayer, targetPlayer.transform.position);
        return TeleportResult.Create(true, "");
    }

    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Chat.Print("Usage: /tp <player>");
            Chat.Print("Usage: /tp <x> <y> <z>");
            Chat.Print("Usage: /tp <player> <x> <y> <z>");
            Chat.Print("Usage: /tp <player> <player>");
            return;
        }

        TeleportResult result;

        switch (args.Length)
        {
            case 1:
                result = this.TeleportToPlayer(args);
                break;
            case 2:
                result = this.TeleportPlayerToPlayer(args);
                break;
            case 3:
                result = this.TeleportToPosition(args);
                break;
            case 4:
                result = this.TeleportPlayerToPosition(args);
                break;
            default:
                result = TeleportResult.Create(false, "Invalid arguments!");
                break;
        }

        if (result.Success)
        {
            Chat.Print(result.Message);
        }
    }
}
