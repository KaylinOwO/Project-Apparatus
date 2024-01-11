using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;

[Command("/mob")]
public class MobCommand : IEnemyPrompter, ICommand
{
    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Chat.Print("Usage: /mob <player>");
            return;
        }

        PlayerControllerB targetPlayer;
        if (!Helper.GetActivePlayer(args[0]).IsNotNull(out targetPlayer))
        {
            Chat.Print("Player not found!");
            return;
        }

        List<string> mobs = this.PromptEnemiesToTarget(targetPlayer, willTeleportEnemies: true);

        if (mobs.Count == 0)
        {
            Chat.Print("No mobs found!");
            return;
        }

        foreach (var enemy in mobs)
        {
            Chat.Print($"{enemy} is in the mob!");
        }
    }
}
