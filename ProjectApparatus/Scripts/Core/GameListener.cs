using System;
using UnityEngine;

namespace Hax
{
    public class GameListener : MonoBehaviour
    {
        public static event Action onGameStart;
        public static event Action onGameEnd;
        public static event Action onShipLand;
        public static event Action onShipLeave;

        private bool InGame { get; set; } = false;
        private bool ShipLand { get; set; } = false;

        void Update()
        {
            InGameListener();
            ShipDoorListener();
        }

        void ShipDoorListener()
        {
            if (!Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound)) return;

            if (startOfRound.shipHasLanded && !ShipLand)
            {
                ShipLand = true;
                onShipLand?.Invoke();
            }
            else if (!startOfRound.shipHasLanded && ShipLand)
            {
                ShipLand = false;
                onShipLeave?.Invoke();
            }
        }

        void InGameListener()
        {
            bool inGame = Helper.LocalPlayer != null;

            if (InGame == inGame)
            {
                return;
            }

            InGame = inGame;

            if (InGame)
            {
                onGameStart?.Invoke();
            }
            else
            {
                onGameEnd?.Invoke();
            }
        }
    }
}
