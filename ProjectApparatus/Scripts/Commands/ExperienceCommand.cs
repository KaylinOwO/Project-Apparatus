using Hax;

[Command("/xp")]
public class ExperienceCommand : ICommand
{
    enum Rank
    {
        INTERN = 0,
        PART_TIME = 1,
        EMPLOYEE = 2,
        LEADER = 3,
        BOSS = 4
    }

    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Chat.Print("Usage: /xp <amount>");
            return;
        }

        if (!int.TryParse(args[0], out int amount))
        {
            Chat.Print("Invalid amount!");
            return;
        }

        if (!Helper.HUDManager.IsNotNull(out HUDManager hudManager))
        {
            Chat.Print("HUDManager is not found");
            return;
        }

        hudManager.localPlayerXP += amount;

        Rank rank;

        if (hudManager.localPlayerXP < 50)
            rank = Rank.INTERN;
        else if (hudManager.localPlayerXP < 100)
            rank = Rank.PART_TIME;
        else if (hudManager.localPlayerXP < 200)
            rank = Rank.EMPLOYEE;
        else if (hudManager.localPlayerXP < 500)
            rank = Rank.LEADER;
        else
            rank = Rank.BOSS;

        hudManager.localPlayerLevel = (int)rank;

        ES3.Save("PlayerXPNum", hudManager.localPlayerXP, "LCGeneralSaveData");
        ES3.Save("PlayerLevel", hudManager.localPlayerLevel, "LCGeneralSaveData");

        hudManager.SyncPlayerLevelServerRpc(
            (int)hudManager.localPlayer.playerClientId,
            hudManager.localPlayerLevel,
            ES3.Load("playedDuringBeta", "LCGeneralSaveData", true)
        );
    }
}
