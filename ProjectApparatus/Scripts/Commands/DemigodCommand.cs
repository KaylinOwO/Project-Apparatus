using Hax;

[Command("/demigod")]
public class DemiGodCommand : ICommand
{
    public void Execute(string[] args)
    {
        Setting.EnableDemigodMode = !Setting.EnableDemigodMode;
        Chat.Print($"Demigod mode: {(Setting.EnableDemigodMode ? "Enabled" : "Disabled")}");
    }
}
