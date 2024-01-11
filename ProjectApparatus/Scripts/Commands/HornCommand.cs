using System;
using ProjectApparatus;
using UnityEngine;

namespace Hax
{

    [Command("/horn")]
    public class HornCommand : ICommand
    {
        Action PullHornLater(float hornDuration) => () =>
        {
            ShipAlarmCord shipAlarmCord = Hax.Helper.FindObject<ShipAlarmCord>();
            if (shipAlarmCord != null)
            {
                shipAlarmCord.PullCordServerRpc(-1);

                var waitForBehaviour = Hax.Helper.CreateComponent<WaitForBehaviour>();
                if (waitForBehaviour != null)
                {
                    waitForBehaviour.SetPredicate(time => time >= hornDuration)
                                   .Init(() => shipAlarmCord.StopPullingCordServerRpc(-1));
                }
            }
        };

        public void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Chat.Print("Usage: /horn <duration>");
                return;
            }

            if (!ulong.TryParse(args[0], out ulong hornDuration))
            {
                Chat.Print("Invalid duration!");
                return;
            }

            Hax.Helper.BuyUnlockable(Unlockable.LOUD_HORN);
            Hax.Helper.ReturnUnlockable(Unlockable.LOUD_HORN);

            var waitForBehaviour = Hax.Helper.CreateComponent<WaitForBehaviour>();
            if (waitForBehaviour != null)
            {
                waitForBehaviour.SetPredicate(time => time >= 0.5f)
                               .Init(PullHornLater((float)hornDuration));
            }
        }
    }
}
