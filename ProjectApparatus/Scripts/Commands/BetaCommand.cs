using Hax;

[Command("/beta")]
public class BetaCommand : ICommand
{
    public void Execute(string[] args)
    {
        bool playedDuringBeta = ES3.Load<bool>("playedDuringBeta", "LCGeneralSaveData", true);
        ES3.Save("playedDuringBeta", !playedDuringBeta, "LCGeneralSaveData");
        Chat.Print($"Beta badge: {(playedDuringBeta ? "obtained" : "removed")}");
    }
}
