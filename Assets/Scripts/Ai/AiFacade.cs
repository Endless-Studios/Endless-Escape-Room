using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Ai
{
    public class AiFacade : MonoBehaviour
    {
        [field: SerializeField] public bool ShouldSpawnOnStart { get; private set; }
    
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private VisualComponent visualComponent;
        [SerializeField] private CollisionComponent collisionComponent;
        [SerializeField] private NavigationComponent navigationComponent;
        [SerializeField] private HearingSensor hearingSensor;
        [SerializeField] private SightSensor sightSensor;
        [SerializeField] private AwarenessComponent awarenessComponent;
        [SerializeField] private NavMeshAgent navMeshAgent;

        public float Health => healthComponent.Health;
        public float MaxHealth => healthComponent.MaxHealth;

        public UnityEvent OnDied;
        public UnityEvent OnSpawn;
        public UnityEvent OnDespawn;

        public void Awake()
        {
            healthComponent.OnDied += () => OnDied?.Invoke();
        }

        [ContextMenu("Spawn")]
        public void Spawn() => OnSpawn?.Invoke();

        [ContextMenu("Despawn")]
        public void Despawn() => OnDespawn?.Invoke();

        public void EnableRenderers() => visualComponent.EnableRenderers();

        public void DisableRenderers() => visualComponent.DisableRenderers();

        public void DisableCollision() => collisionComponent.DisableCollision();

        public void EnableCollision() => collisionComponent.EnableCollision();

        public void SetHealthToMax() => healthComponent.SetHealthToMax();
    }
}