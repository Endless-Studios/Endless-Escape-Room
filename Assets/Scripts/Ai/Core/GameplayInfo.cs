using System.Collections;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This class holds values that change during gameplay, are set by the Ai for implementing its behavior, and need to be accessed by multiple classes or visual scripting
    /// nodes. This component should be placed on the root game object of the Ai prefab. 
    /// </summary>
    public class GameplayInfo : MonoBehaviour
    {
        [SerializeField] private float targetMemoryTime;
        [ShowOnly] public PointOfInterest CurrentPointOfInterest;
        [ShowOnly] public Hideout TargetHideout;
        [ShowOnly, SerializeField] private PlayerTarget target;
        [ShowOnly, SerializeField] private PlayerTarget recentTarget;
        [ShowOnly] public Hideout PlayersHideout;
        [ShowOnly] public Vector3 Destination;
        [ShowOnly] public AlertState AlertState;
        [HideInInspector] public bool ShouldSpawnInPlace;
        
        public Stimulus CurrentStimulus;
        public Vector3 InitialSpawnPoint { get; private set; }
        public PlayerTarget Target
        {
            get => target;
            set
            {
                if (value is null)
                {
                    recentTarget = target;
                    StartCoroutine(ForgetTargetRoutine());
                    target = null;
                }
                else
                {
                    target = value;
                }
            }
        }

        public PlayerTarget RecentTarget => recentTarget;

        private void Awake()
        {
            InitialSpawnPoint = transform.position;
        }

        /// <summary>
        /// Resets all Gameplay values to their defaults. 
        /// </summary>
        public void ResetValues()
        {
            CurrentPointOfInterest = null;
            TargetHideout = null;
            target = null;
            PlayersHideout = null;
            Destination = transform.position;
            CurrentStimulus = null;
            AlertState = AlertState.Unaware;
            recentTarget = null;
        }

        private IEnumerator ForgetTargetRoutine()
        {
            yield return new WaitForSeconds(targetMemoryTime);
            recentTarget = null;
        }
    }
}