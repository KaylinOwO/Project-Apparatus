using System;
using GameNetcodeStuff;
using Hax;

[Command("/home")]
public class HomeCommand : ITeleporter, ICommand
{
    private Action TeleportPlayerToBaseLater(PlayerControllerB targetPlayer)
    {
        return () =>
        {
            HaxObjects.Instance?.ShipTeleporters.Renew();

            ShipTeleporter teleporter;
            if (!this.TryGetTeleporter(out teleporter))
            {
                Chat.Print("ShipTeleporter not found!");
                return;
            }

            Helper.SwitchRadarTarget(targetPlayer);
            Helper.CreateComponent<WaitForBehaviour>()
                  .SetPredicate(_ => Helper.IsRadarTarget(targetPlayer.playerClientId))
                  .Init(teleporter.PressTeleportButtonServerRpc);
        };
    }

    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Helper.StartOfRound?.ForcePlayerIntoShip();
            return;
        }

        PlayerControllerB targetPlayer;
        if (!Helper.GetPlayer(args[0]).IsNotNull(out targetPlayer))
        {
            Chat.Print("Player not found!");
            return;
        }

        this.PrepareToTeleport(this.TeleportPlayerToBaseLater(targetPlayer));
    }
}
