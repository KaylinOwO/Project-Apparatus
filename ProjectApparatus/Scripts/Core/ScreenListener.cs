using System;
using UnityEngine;

namespace Hax
{
    public class ScreenListener : MonoBehaviour
    {
        public static event Action onScreenSizeChange;

        private int LastScreenWidth { get; set; } = Screen.width;
        private int LastScreenHeight { get; set; } = Screen.height;

        void Update()
        {
            ScreenSizeListener();
        }

        void ScreenSizeListener()
        {
            if (Screen.width == LastScreenWidth && Screen.height == LastScreenHeight) return;

            LastScreenWidth = Screen.width;
            LastScreenHeight = Screen.height;
            onScreenSizeChange?.Invoke();
        }
    }
}
