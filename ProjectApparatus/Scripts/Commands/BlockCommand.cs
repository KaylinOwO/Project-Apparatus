using Hax;

[Command("/block")]
public class BlockCommand : ICommand
{
    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Chat.Print("Usage: /block <property>");
            return;
        }

        string property = args[0].ToLowerInvariant(); // Convert to lowercase for case-insensitive comparison

        if (property == "credits")
        {
            Setting.EnableBlockCredits = !Setting.EnableBlockCredits;

            Chat.Print($"{(Setting.EnableBlockCredits ? "Blocking all incoming credits!" : "No longer blocking credits!")}");
        }
        else if (property == "enemy")
        {
            Setting.EnableUntargetable = !Setting.EnableUntargetable;

            Chat.Print($"{(Setting.EnableUntargetable ? "Enemies will no longer target you!" : "Enemies can now target you!")}");
        }
        else if (property == "radar")
        {
            Setting.EnableBlockRadar = !Setting.EnableBlockRadar;

            Chat.Print($"{(Setting.EnableBlockRadar ? "Blocking radar targets!" : "No longer blocking radar targets!")}");
        }
        else
        {
            Chat.Print($"Invalid property!");
        }
    }
}
