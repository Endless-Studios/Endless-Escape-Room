using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
// ReSharper disable UnusedMember.Global

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
        [SerializeField] private WanderComponent wanderComponent;
        [SerializeField] private FidgetComponent fidgetComponent;
        [SerializeField] private PatrolComponent patrolComponent;

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

        public float NavigationTolerance => navigationComponent.NavigationTolerance;
        
        public bool HasDestination => navigationComponent.HasDestination;

        public Vector3 Destination => navigationComponent.Destination;

        public void UpdateSenses() => sightSensor.CheckLos();

        public float WanderThreshold => wanderComponent.WanderThreshold;

        public void StartWandering() => wanderComponent.StartWandering();

        public float TimeSinceLastWander => wanderComponent.TimeSinceLastWander;
        
        public void ResetLastWanderTime() => wanderComponent.ResetLastWanderTime();

        public float FidgetThreshold => fidgetComponent.FidgetThreshold;

        public float TimeSinceLastFidget => fidgetComponent.TimeSinceLastFidget;
        
        public void ResetLastFidgetTime() => fidgetComponent.ResetLastFidgetTime();

        public void StartFidgeting() => fidgetComponent.StartFidgeting();

        public bool IsFidgeting => fidgetComponent.IsFidgeting;

        public void FidgetInterrupted() => fidgetComponent.StopFidgeting();

        public float PatrolThreshold => patrolComponent.PatrolThreshold;

        public float TimeSinceLastPatrol => patrolComponent.TimeSinceLastPatrol;

        public void ResetLastPatrolTime() => patrolComponent.ResetLastPatrolTime();

        public void StartPatrolling() => patrolComponent.StartPatrolling();

        public void UpdateBoredom(float deltaTime)
        {
            wanderComponent.UpdateLastWanderTime(deltaTime);
            fidgetComponent.UpdateLastFidgetTime(deltaTime);
            patrolComponent.UpdateLastPatrolTime(deltaTime);
        }
    }
}