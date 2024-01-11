using System;
using System.Collections;
using UnityEngine;

namespace Hax
{
    public class TransientBehaviour : MonoBehaviour
    {
        private Action<float> Action { get; set; }
        private Action DisposeAction { get; set; }
        private float ExpireTime { get; set; } = 0.0f;

        public TransientBehaviour Init(Action<float> action, float expireTime, float delay = 0.0f)
        {
            Action = action;
            ExpireTime = expireTime;

            _ = delay > 0.0f
                ? StartCoroutine(TransientCoroutine(delay))
                : StartCoroutine(TransientCoroutine());

            return this;
        }

        public void Dispose(Action disposeAction)
        {
            DisposeAction = disposeAction;
        }

        private IEnumerator TransientCoroutine(float delay)
        {
            while (ExpireTime > 0.0f)
            {
                float deltaTime = Time.deltaTime;
                ExpireTime -= deltaTime;
                Action?.Invoke(deltaTime);

                yield return new WaitForSeconds(delay);
            }

            if (DisposeAction != null)
                DisposeAction.Invoke();

            Destroy(gameObject);
        }

        private IEnumerator TransientCoroutine()
        {
            while (ExpireTime > 0.0f)
            {
                float deltaTime = Time.deltaTime;
                ExpireTime -= deltaTime;
                Action?.Invoke(deltaTime);

                yield return new WaitForEndOfFrame();
            }

            if (DisposeAction != null)
                DisposeAction.Invoke();

            Destroy(gameObject);
        }
    }
}
