using System;
using System.Collections.Generic;

namespace Ai
{
    public class Hideout : PointOfInterest
    {
        public static readonly List<Hideout> Hideouts = new List<Hideout>();
        public static event Action<Hideout> OnEnteredHideout;
        public static event Action<Hideout> OnLeftHideout;
        
        protected override void Awake()
        {
            base.Awake();
            Hideouts.Add(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Hideouts.Remove(this);
        }
    }
}