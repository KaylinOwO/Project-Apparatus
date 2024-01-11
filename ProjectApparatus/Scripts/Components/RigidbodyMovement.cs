using UnityEngine;
using GameNetcodeStuff;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Hax
{
    public class RigidbodyMovement : MonoBehaviour
    {
        private const float baseSpeed = 25.0f;
        private const float jumpForce = 12.0f;

        private Rigidbody rigidbody;
        private SphereCollider sphereCollider;

        private float SprintMultiplier { get; set; } = 1.0f;
        private List<Collider> CollidedColliders { get; } = new List<Collider>();

        public void Init()
        {
            if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) return;
            gameObject.layer = localPlayer.gameObject.layer;
        }

        private void Awake()
        {
            rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 0.25f;
        }

        private void OnEnable()
        {
            rigidbody.isKinematic = false;
        }

        private void OnDisable()
        {
            rigidbody.isKinematic = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            CollidedColliders.Add(collision.collider);
        }

        private void OnCollisionExit(Collision collision)
        {
            _ = CollidedColliders.Remove(collision.collider);
        }

        private void Update()
        {
            if (!Keyboard.current.IsNotNull(out Keyboard keyboard)) return;

            Vector3 direction = new Vector3(
                keyboard.dKey.ReadValue() - keyboard.aKey.ReadValue(),
                keyboard.spaceKey.ReadValue() - keyboard.ctrlKey.ReadValue(),
                keyboard.wKey.ReadValue() - keyboard.sKey.ReadValue()
            );

            UpdateSprintMultiplier(keyboard);
            Move(direction);

            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                Jump();
            }

            if (keyboard.spaceKey.isPressed)
            {
                BunnyHop();
            }
        }

        private void UpdateSprintMultiplier(Keyboard keyboard)
        {
            SprintMultiplier = keyboard.shiftKey.IsPressed()
                ? Mathf.Min(SprintMultiplier + (5.0f * Time.deltaTime), 5.0f)
                : 1.0f;
        }

        private void Move(Vector3 direction)
        {
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            forward.y = 0.0f;
            right.y = 0.0f;
            forward = forward.normalized;
            right = right.normalized;

            Vector3 translatedDirection = (right * direction.x) + (forward * direction.z);
            rigidbody.velocity += translatedDirection * Time.deltaTime * baseSpeed * SprintMultiplier;
        }

        private void Jump()
        {
            Vector3 newVelocity = rigidbody.velocity;
            newVelocity.y = jumpForce;
            rigidbody.velocity = newVelocity;
        }

        private void BunnyHop()
        {
            if (CollidedColliders.Count <= 0) return;
            Jump();
        }
    }
}
