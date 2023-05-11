using System;
using UnityEngine;
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
        [SerializeField] private WanderNearComponent wanderNearComponent;
        [SerializeField] private FidgetComponent fidgetComponent;
        [SerializeField] private WanderFarComponent wanderFarComponent;
        [SerializeField] private AnimationComponent animationComponent;
        
        public UnityEvent OnDied;
        public UnityEvent OnSpawn;
        public UnityEvent OnDespawn;

        public event Action OnWalkingThroughDoorway;

        public event Action OnWalkedThroughDoorway;

        public void WalkingThroughDoorway() => OnWalkingThroughDoorway?.Invoke();

        public void WalkedThroughDoorway() => OnWalkedThroughDoorway?.Invoke();

        public float Health => healthComponent.Health;
        public float MaxHealth => healthComponent.MaxHealth;

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

        public Vector3 Destination
        {
            get => navigationComponent.Destination;
            set => navigationComponent.Destination = value;
        }

        public Room CurrentRoom => navigationComponent.CurrentRoom;

        public Room LastRoom => navigationComponent.LastRoom;

        public void UpdateSenses() => sightSensor.CheckLos();

        public float WanderThreshold => wanderNearComponent.WanderThreshold;

        public void StartWandering() => wanderNearComponent.StartWandering();

        public float TimeSinceLastWander => wanderNearComponent.TimeSinceLastWander;
        
        public void ResetLastWanderTime() => wanderNearComponent.ResetLastWanderTime();

        public float FidgetThreshold => fidgetComponent.FidgetThreshold;

        public float TimeSinceLastFidget => fidgetComponent.TimeSinceLastFidget;
        
        public void ResetLastFidgetTime() => fidgetComponent.ResetLastFidgetTime();

        public void StartFidgeting() => fidgetComponent.StartFidgeting();

        public bool IsFidgeting => fidgetComponent.IsFidgeting;

        public void FidgetInterrupted() => fidgetComponent.StopFidgeting();

        public float PatrolThreshold => wanderFarComponent.PatrolThreshold;

        public float TimeSinceLastPatrol => wanderFarComponent.TimeSinceLastPatrol;

        public void ResetLastWanderFarTime() => wanderFarComponent.ResetLastWanderFarTime();

        public void StartWanderingFar() => wanderFarComponent.StartWandering();

        public bool IsMoving => navigationComponent.IsMoving;

        public void UpdateBoredom(float deltaTime)
        {
            wanderNearComponent.UpdateLastWanderTime(deltaTime);
            fidgetComponent.UpdateLastFidgetTime(deltaTime);
            wanderFarComponent.UpdateLastPatrolTime(deltaTime);
        }
        
        private Room currentRoom;
    }
}