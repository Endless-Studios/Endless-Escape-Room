using UnityEngine;

namespace Ai
{
    /// <summary>
    /// Component for invoking hideout events from a trigger volume.
    /// </summary>
    public class HideoutVolume : MonoBehaviour
    {
        [SerializeField] private Hideout hideout;

        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponentInParent<PlayerCore>())
                hideout.PlayerEnteredHideout();
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.GetComponentInParent<PlayerCore>())
                hideout.PlayerLeftHideout();
        }
    }
}