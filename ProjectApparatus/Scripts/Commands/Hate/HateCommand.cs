using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;

[Command("/hate")]
public class HateCommand : IEnemyPrompter, ICommand
{
    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Chat.Print("Usage: /hate <player> <funnyRevive>");
            return;
        }

        PlayerControllerB targetPlayer;
        if (!Helper.GetActivePlayer(args[0]).IsNotNull(out targetPlayer))
        {
            Chat.Print("Player not found!");
            return;
        }

        List<string> promptedEnemies = this.PromptEnemiesToTarget(targetPlayer);

        if (promptedEnemies.Count == 0)
        {
            Chat.Print("No enemies found!");
            return;
        }

        foreach (var enemy in promptedEnemies)
        {
            Chat.Print($"{enemy} prompted!");
        }

        Chat.Print($"Enemies prompted: {promptedEnemies.Count}");
    }
}
