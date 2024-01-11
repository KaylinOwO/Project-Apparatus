using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hax
{
    public class InputListener : MonoBehaviour
    {
        public static event Action<bool> onShiftButtonHold;
        public static event Action<bool> onFButtonHold;
        public static event Action<bool> onEButtonHold;
        public static event Action<bool> onRButtonHold;
        public static event Action onMiddleButtonPress;
        public static event Action onLeftButtonPress;
        public static event Action onLeftArrowKeyPress;
        public static event Action onRightArrowKeyPress;
        public static event Action onEqualsPress;
        public static event Action onLeftBracketPress;
        public static event Action onRightBracketPress;
        public static event Action onBackslashPress;
        public static event Action onZPress;
        public static event Action onXPress;
        public static event Action onNPress;

        private Dictionary<Func<bool>, Action> InputActions { get; } = new Dictionary<Func<bool>, Action>
        {
            { () => Mouse.current.middleButton.wasPressedThisFrame, () => onMiddleButtonPress?.Invoke() },
            { () => Mouse.current.leftButton.wasPressedThisFrame, () => onLeftButtonPress?.Invoke() },
            { () => true, () => onShiftButtonHold?.Invoke(Keyboard.current[Key.LeftShift].isPressed) },
            { () => true, () => onFButtonHold?.Invoke(Keyboard.current[Key.F].isPressed) },
            { () => true, () => onRButtonHold?.Invoke(Keyboard.current[Key.R].isPressed) },
            { () => true, () => onEButtonHold?.Invoke(Keyboard.current[Key.E].isPressed) },
            { () => Keyboard.current[Key.Equals].wasPressedThisFrame, () => onEqualsPress?.Invoke() },
            { () => Keyboard.current[Key.LeftArrow].wasPressedThisFrame, () => onLeftArrowKeyPress?.Invoke() },
            { () => Keyboard.current[Key.RightArrow].wasPressedThisFrame, () => onRightArrowKeyPress?.Invoke() },
            { () => Keyboard.current[Key.LeftBracket].wasPressedThisFrame, () => onLeftBracketPress?.Invoke() },
            { () => Keyboard.current[Key.RightBracket].wasPressedThisFrame, () => onRightBracketPress?.Invoke() },
            { () => Keyboard.current[Key.Backslash].wasPressedThisFrame, () => onBackslashPress?.Invoke() },
            { () => Keyboard.current[Key.Z].wasPressedThisFrame, () => onZPress?.Invoke() },
            { () => Keyboard.current[Key.X].wasPressedThisFrame, () => onXPress?.Invoke() },
            { () => Keyboard.current[Key.N].wasPressedThisFrame, () => onNPress?.Invoke() },
        };

        void Update()
        {
            if (Helper.LocalPlayer.IsNotNull(out PlayerControllerB player) && player.isTypingChat) return;

            foreach (KeyValuePair<Func<bool>, Action> keyAction in InputActions)
            {
                if (!keyAction.Key()) continue;
                keyAction.Value();
            }
        }
    }
}
