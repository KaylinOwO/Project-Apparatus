using Hax;

public class ExplodeCommand : ICommand
{
    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            foreach (var jetpack in Helper.FindObjects<JetpackItem>())
            {
                jetpack.ExplodeJetpackServerRpc();
            }
        }

        if (args.Length > 0 && args[0] == "mine")
        {
            foreach (var landmine in Helper.FindObjects<Landmine>())
            {
                landmine.TriggerMine();
            }
        }
    }
}
