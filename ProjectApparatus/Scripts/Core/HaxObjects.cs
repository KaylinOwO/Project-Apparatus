using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Hax
{
    public class HaxObjects : MonoBehaviour
    {
        public static HaxObjects Instance { get; private set; }

        public SingleObjectPool<DepositItemsDesk> DepositItemsDesk { get; private set; }
        public MultiObjectPool<ShipTeleporter> ShipTeleporters { get; private set; }
        public MultiObjectPool<LocalVolumetricFog> LocalVolumetricFogs { get; private set; }
        public MultiObjectPool<SteamValveHazard> SteamValves { get; private set; }
        public MultiObjectPool<InteractTrigger> InteractTriggers { get; private set; }
        public MultiObjectPool<EnemyAI> EnemyAIs { get; private set; }

        void Awake()
        {
            DepositItemsDesk = new SingleObjectPool<DepositItemsDesk>(this);
            ShipTeleporters = new MultiObjectPool<ShipTeleporter>(this);
            LocalVolumetricFogs = new MultiObjectPool<LocalVolumetricFog>(this);
            InteractTriggers = new MultiObjectPool<InteractTrigger>(this);
            SteamValves = new MultiObjectPool<SteamValveHazard>(this, 5.0f);
            EnemyAIs = new MultiObjectPool<EnemyAI>(this, 2.0f);

            Instance = this;
        }
    }
}
