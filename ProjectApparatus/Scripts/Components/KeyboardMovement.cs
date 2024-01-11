using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hax
{
    public class KeyboardMovement : MonoBehaviour
    {
        private const float baseSpeed = 20;
        private float sprintMultiplier = 1;

        private void Update()
        {
            if (Helper.LocalPlayer.IsNotNull(out PlayerControllerB player) && player.isTypingChat)
                return;

            if (!Keyboard.current.IsNotNull(out Keyboard keyboard))
                return;

            Vector3 direction = new Vector3(
                keyboard.dKey.ReadValue() - keyboard.aKey.ReadValue(),
                keyboard.spaceKey.ReadValue() - keyboard.ctrlKey.ReadValue(),
                keyboard.wKey.ReadValue() - keyboard.sKey.ReadValue()
            );

            UpdateSprintMultiplier(keyboard);
            Move(direction);
        }

        private void UpdateSprintMultiplier(Keyboard keyboard)
        {
            sprintMultiplier = keyboard.shiftKey.IsPressed()
                ? Mathf.Min(sprintMultiplier + (5 * Time.deltaTime), 5)
                : 1;
        }

        private void Move(Vector3 direction)
        {
            Vector3 translatedDirection =
                (transform.right * direction.x) +
                (transform.up * direction.y) +
                (transform.forward * direction.z);

            transform.position += translatedDirection * Time.deltaTime * baseSpeed * sprintMultiplier;
        }
    }
}
