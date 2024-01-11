using UnityEngine;

namespace Hax
{
    public static partial class Helper
    {
        public static Camera CurrentCamera
        {
            get
            {
                if (Helper.LocalPlayer?.gameplayCamera.IsNotNull(out Camera gameplayCamera) == true &&
                    gameplayCamera.enabled)
                {
                    return gameplayCamera;
                }
                else
                {
                    return Helper.StartOfRound?.spectateCamera;
                }
            }
        }
    }
}
