using Hax;

[Command("/berserk")]
public class BerserkCommand : ICommand
{
    public void Execute(string[] args)
    {
        var turrets = Helper.FindObjects<Turret>();

        foreach (var turret in turrets)
        {
            turret.EnterBerserkModeServerRpc(-1);
        }
    }
}
