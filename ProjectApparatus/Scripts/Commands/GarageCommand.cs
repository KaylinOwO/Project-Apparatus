using Hax;

[Command("/garage")]
public class GarageCommand : ICommand
{
    public void Execute(string[] _)
    {
        if (!Helper.RoundManager.IsNotNull(out RoundManager roundManager))
        {
            Chat.Print("RoundManager not found!");
            return;
        }

        if (!(roundManager.currentLevel.levelID is int levelId) || levelId != (int)Level.EXPERIMENTATION)
        {
            Chat.Print("You must be in Experimentation to use this command!");
            return;
        }

        HaxObjects.Instance?.InteractTriggers.ForEach(nullableIteractTrigger => {
            if (!(nullableIteractTrigger.IsNotNull(out InteractTrigger interactTrigger))) return;
            if (!(interactTrigger.name is "Cube" && interactTrigger.transform.parent.name is "Cutscenes")) return;

            interactTrigger.randomChancePercentage = 100;
            interactTrigger.Interact(Helper.LocalPlayer?.transform);
        });
    }
}
