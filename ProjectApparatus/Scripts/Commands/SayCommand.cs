using GameNetcodeStuff;
using Hax;
using System.Linq;

[Command("/say")]
public class SayCommand : ICommand
{
    public void Execute(string[] args)
    {
        if (args.Length < 2)
        {
            Chat.Print("Usage: /say <player> <message>");
            return;
        }

        if (!Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB player))
        {
            Chat.Print("Player not found!");
            return;
        }

        // Combine all elements from index 1 to the end to form the message
        string message = string.Join(" ", args.Skip(1).ToArray());

        Helper.HUDManager?.AddTextToChatOnServer(message, (int)player.playerClientId);
    }
}
