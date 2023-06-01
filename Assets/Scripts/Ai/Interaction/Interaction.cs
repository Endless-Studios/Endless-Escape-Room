namespace Ai
{
    /// <summary>
    /// A pair of values containing a point of interest and the time that it was interacted with.
    /// </summary>
    public readonly struct Interaction
    {
        public readonly PointOfInterest PointOfInterest;
        public readonly float TimeOfInteraction;

        public Interaction(PointOfInterest pointOfInterest, float timeOfInteraction)
        {
            PointOfInterest = pointOfInterest;
            TimeOfInteraction = timeOfInteraction;
        }
    }
}