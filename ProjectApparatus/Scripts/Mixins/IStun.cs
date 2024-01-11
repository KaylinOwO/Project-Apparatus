using UnityEngine;

namespace Hax;

public interface IStun { }

public static class IStunMixin {
    public static void Stun(this IStun _, Vector3 position, float radius, float duration = 1.0f) =>
        Physics.OverlapSphere(position, radius, 524288)
               .ForEach(collider => {
                   if (!collider.TryGetComponent(out EnemyAICollisionDetect enemy)) return;
                   enemy.mainScript.SetEnemyStunned(true, duration);
               });
}
