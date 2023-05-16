using UnityEngine;

namespace Sound
{
    /// <summary>
    /// This component can be placed on any layer that blocks sound and will override the default sound blocking value
    /// for that collider.
    /// </summary>
    public class SoundBlockingModifier : MonoBehaviour
    {
        [Range(0, 80)] public float SoundBlockingValue;
    }
    
}