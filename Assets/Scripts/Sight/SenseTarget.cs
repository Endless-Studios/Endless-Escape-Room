using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sight
{
    
    public class SenseTarget : MonoBehaviour
    {
        public static readonly List<SenseTarget> SenseTargets = new List<SenseTarget>();
        
        [field: SerializeField] public List<LosProbe> LosProbes { get; private set; }

        private void Awake()
        {
            SenseTargets.Add(this);
        }

        private void OnDestroy()
        {
            SenseTargets.Remove(this);
        }
    }
}