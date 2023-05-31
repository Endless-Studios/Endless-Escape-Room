using System;

namespace Ai
{
    /// <summary>
    /// A pair of values containing a point of interest and the distance to that point from an Ais
    /// current position. Easily sorted by the distance.
    /// </summary>
    public readonly struct PointOfInterestDistancePair : IComparable
    {
        public readonly PointOfInterest PointOfInterest;
        public readonly float Distance;

        public PointOfInterestDistancePair(PointOfInterest pointOfInterest, float distance)
        {
            PointOfInterest = pointOfInterest;
            Distance = distance;
        }

        public int CompareTo(object obj)
        {
            PointOfInterestDistancePair otherPair = (PointOfInterestDistancePair)obj;
            return Distance.CompareTo(otherPair.Distance);
        }
    }
}