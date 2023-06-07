using System;
using System.Collections.Generic;
using Sight;
using UnityEngine;

namespace Ai
{
    
    public class PlayerTarget : MonoBehaviour
    {
        public static readonly List<PlayerTarget> SenseTargets = new List<PlayerTarget>();
        
        [field: SerializeField] public List<LosProbe> LosProbes { get; private set; }

        private void Awake()
        {
            SenseTargets.Add(this);
        }

        private void OnDestroy()
        {
            SenseTargets.Remove(this);
        }

        public void StartFadeout(Action fadeoutCompleteCallback)
        {
            
        }

        public void DealDamage(float damage)
        {
            
        }
    }
}