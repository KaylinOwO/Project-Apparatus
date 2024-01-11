using Hax;

[Command("/players")]
public class PlayersCommand : ICommand {
    public void Execute(string[] args) {
        Helper.StartOfRound?.allPlayerScripts.ForEach(player =>
            Chat.Print($"{player.playerUsername} ({player.playerClientId})")
        );
    }
}
