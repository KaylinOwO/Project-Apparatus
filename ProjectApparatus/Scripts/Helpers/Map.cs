#nullable enable

using GameNetcodeStuff;

namespace Hax
{
    public static partial class Helper
    {
        private static ManualCameraRenderer? GetManualCameraRenderer()
        {
            return StartOfRound != null ? StartOfRound.mapScreen : null;
        }

        private static ManualCameraRenderer? ManualCameraRenderer
        {
            get { return GetManualCameraRenderer(); }
        }

        public static void SwitchRadarTarget(int playerClientId)
        {
            if (ManualCameraRenderer != null)
            {
                ManualCameraRenderer.SwitchRadarTargetServerRpc(playerClientId);
            }
        }

        public static void SwitchRadarTarget(PlayerControllerB player)
        {
            SwitchRadarTarget((int)player.playerClientId);
        }

        public static bool IsRadarTarget(ulong playerClientId)
        {
            return ManualCameraRenderer?.targetTransformIndex == (int)playerClientId;
        }
    }
}
