using GameNetcodeStuff;
using Hax;

[Command("/heal")]
public class HealCommand : IStun, ICommand
{
    void StunAtPlayerPosition(PlayerControllerB player)
    {
        this.Stun(player.transform.position, 5.0f, 1.0f);
    }

    Result HealLocalPlayer()
    {
        HUDManager hudManager;
        if (!Helper.HUDManager.IsNotNull(out hudManager))
        {
            return new Result(false, "HUDManager is not found");

        }

        hudManager.localPlayer.health = 100;
        hudManager.localPlayer.bleedingHeavily = false;
        hudManager.localPlayer.criticallyInjured = false;
        hudManager.localPlayer.hasBeenCriticallyInjured = false;
        hudManager.localPlayer.playerBodyAnimator.SetBool("Limp", false);
        hudManager.HUDAnimator.SetBool("biohazardDamage", false);
        hudManager.HUDAnimator.SetTrigger("HealFromCritical");
        hudManager.UpdateHealthUI(hudManager.localPlayer.health, false);

        this.StunAtPlayerPosition(hudManager.localPlayer);
        return new Result(true);
    }

    Result HealPlayer(string[] args)
    {
        PlayerControllerB targetPlayer;
        if (args.Length == 0 || !Helper.GetActivePlayer(args[0]).IsNotNull(out targetPlayer))
        {
            return new Result(false, "Player not found!");

        }

        targetPlayer.HealPlayer();
        this.StunAtPlayerPosition(targetPlayer);

        return new Result(true);
    }

    public void Execute(string[] args)
    {
        Result result = args.Length == 0 ? this.HealLocalPlayer() : this.HealPlayer(args);

        if (!result.Success)
        {
            Chat.Print(result.Message);
        }
    }
}
