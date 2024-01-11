using GameNetcodeStuff;
using Hax;

[Command("/void")]
public class VoidCommand : ITeleporter, ICommand
{
    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Chat.Print("Usage: /void <player>");
            return;
        }

        PlayerControllerB player;
        if (!Helper.GetActivePlayer(args[0]).IsNotNull(out player))
        {
            Chat.Print("Player not found!");
            return;
        }

        this.PrepareToTeleport(this.TeleportPlayerToPositionLater(
            player,
            player.playersManager.notSpawnedPosition.position
        ));
    }
}
