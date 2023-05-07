using UnityEngine;

namespace Sight
{
    public class LosProbe : MonoBehaviour
    {
        [field: SerializeField] public ProbeEnum ProbeKind { get; private set; }
        [field: SerializeField] public Collider LosCollider { get; private set; }
        
        public enum ProbeEnum
        {
            Head,
            Body,
            Foot,
            Hand
        }

        public static float GetLosScalarByProbeKind(ProbeEnum probeKind)
        {
            return probeKind switch
            {
                ProbeEnum.Head => 1f,
                ProbeEnum.Body => 1f,
                ProbeEnum.Foot => .5f,
                ProbeEnum.Hand => .5f,
                _ => 0f
            };
        }
    }
}