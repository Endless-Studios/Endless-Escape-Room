using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sight
{
    public class SightTarget : MonoBehaviour
    {
        public static readonly List<SightTarget> SightTargets = new();
        
        [field: SerializeField] public List<LosProbe> LosProbes { get; private set; }

        private void Awake()
        {
            SightTargets.Add(this);
        }

        private void OnDestroy()
        {
            SightTargets.Remove(this);
        }
    }
}