using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sight
{
    public class SightTarget : MonoBehaviour
    {
        public static readonly List<SightTarget> SightTargets = new();
        
        [SerializeField] private List<Transform> losProbes = new();

        public List<Vector3> GetLosProbeLocations()
        {
            return losProbeLocations;
        }

        private void Awake()
        {
            SightTargets.Add(this);
        }

        private void OnDestroy()
        {
            SightTargets.Remove(this);
        }

        private readonly List<Vector3> losProbeLocations = new();
    }
}