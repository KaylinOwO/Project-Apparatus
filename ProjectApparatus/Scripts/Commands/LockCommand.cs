using Hax;

[Command("/lock")]
public class LockCommand : ICommand {
    public void Execute(string[] args) {
        Helper.SetGateState(false);
    }
}
