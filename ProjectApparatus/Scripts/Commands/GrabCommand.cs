using GameNetcodeStuff;
using UnityEngine;
using Hax;

public class GrabCommand : ICommand
{
    public void Execute(string[] args)
    {
        ShipBuildModeManager shipBuildModeManager;
        if (!Helper.ShipBuildModeManager.IsNotNull(out shipBuildModeManager))
        {
            Chat.Print("ShipBuildModeManager not found!");
            return;
        }

        PlayerControllerB localPlayer;
        if (!Helper.LocalPlayer.IsNotNull(out localPlayer))
        {
            Chat.Print("Player not found!");
            return;
        }

        Vector3 currentPlayerPosition = localPlayer.transform.position;
        Vector3 microOffset = localPlayer.transform.forward + localPlayer.transform.up;
        Vector3 positionOffset = currentPlayerPosition - shipBuildModeManager.transform.position + microOffset;

        foreach (GrabbableObject grabbableObject in Helper.FindObjects<GrabbableObject>())
        {
            if (Vector3.Distance(grabbableObject.transform.position, currentPlayerPosition) < 20.0f)
                continue;

            if (localPlayer != null)
            {
                localPlayer.PlaceGrabbableObject(
                    shipBuildModeManager.transform,
                    positionOffset,
                    true,
                    grabbableObject
                );
            }
        }
    }
}
