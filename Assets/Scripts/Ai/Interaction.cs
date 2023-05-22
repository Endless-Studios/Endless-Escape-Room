namespace Ai
{
    public struct Interaction
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