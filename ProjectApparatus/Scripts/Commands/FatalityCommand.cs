using GameNetcodeStuff;
using Hax;

public class FatalityCommand : ICommand
{
    T GetEnemy<T>() where T : EnemyAI
    {
        T enemy = Helper.FindObject<T>() as T;

        if (enemy != null && Helper.LocalPlayer is PlayerControllerB localPlayer)
        {
            enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
        }

        return enemy;
    }

    Result HandleGiant(PlayerControllerB targetPlayer)
    {
        ForestGiantAI forestGiant = this.GetEnemy<ForestGiantAI>() as ForestGiantAI;

        if (forestGiant == null)
        {
            return new Result(false, "Enemy has not yet spawned!");
        }

        forestGiant.GrabPlayerServerRpc((int)targetPlayer.playerClientId);
        return new Result(true);
    }

    Result HandleJester(PlayerControllerB targetPlayer)
    {
        JesterAI jester = this.GetEnemy<JesterAI>() as JesterAI;

        if (jester == null)
        {
            return new Result(false, "Enemy has not yet spawned!");
        }

        jester.KillPlayerServerRpc((int)targetPlayer.playerClientId);
        return new Result(true);
    }

    Result HandleMask(PlayerControllerB targetPlayer)
    {
        MaskedPlayerEnemy spider = this.GetEnemy<MaskedPlayerEnemy>() as MaskedPlayerEnemy;

        if (spider == null)
        {
            return new Result(false, "Enemy has not yet spawned!");
        }

        spider.KillPlayerAnimationServerRpc((int)targetPlayer.playerClientId);
        return new Result(true);
    }

    public void Execute(string[] args)
    {
        if (args.Length < 2)
        {
            Chat.Print("Usage: /fatality <player> <enemy>");
            return;
        }

        PlayerControllerB targetPlayer = Helper.GetActivePlayer(args[0]) as PlayerControllerB;

        if (targetPlayer == null)
        {
            Chat.Print($"Unable to find player: {args[0]}!");
            return;
        }

        Result result = default;

        switch (args[1].ToLower())
        {
            case "giant":
                result = this.HandleGiant(targetPlayer);
                break;
            case "jester":
                result = this.HandleJester(targetPlayer);
                break;
            case "mask":
                result = this.HandleMask(targetPlayer);
                break;
            default:
                Chat.Print("Enemy has either not yet been implemented or does not exist!");
                break;
        }

        if (!result.Success)
        {
            Chat.Print(result.Message);
        }
    }
}
