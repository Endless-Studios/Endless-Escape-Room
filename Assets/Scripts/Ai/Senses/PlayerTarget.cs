using System;
using System.Collections;
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
            StartCoroutine(fauxFadeout(fadeoutCompleteCallback));
        }

        private IEnumerator fauxFadeout(Action fadeoutCompleteCallback)
        {
            yield return new WaitForSeconds(2f);
            fadeoutCompleteCallback.Invoke();
        }

        public void DealDamage(float damage)
        {
            
        }
    }
}