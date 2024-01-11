using System;
using System.Collections;
using UnityEngine;

namespace Hax
{
    public class WaitForBehaviour : MonoBehaviour
    {
        private Action Action { get; set; }
        private Func<float, bool> Predicate { get; set; }

        public void Init(Action action)
        {
            this.Action = action;
            StartCoroutine(WaitForPredicateCoroutine());
        }

        public WaitForBehaviour SetPredicate(Func<float, bool> predicate)
        {
            this.Predicate = predicate;
            return this;
        }

        private IEnumerator WaitForPredicateCoroutine()
        {
            float timer = 0.0f;

            while (true)
            {
                if (this.Predicate != null && this.Predicate(timer)) break;
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (this.Action != null)
            {
                this.Action.Invoke();
            }

            Destroy(this.gameObject);
        }
    }
}
