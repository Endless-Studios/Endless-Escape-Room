using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This component manages the NavMeshLink on a specific threshold and handles disabling and enabling the link based
    /// on gameplay events. 
    /// </summary>
    public class LinkController : MonoBehaviour
    {
        [SerializeField] private NavMeshLink navMeshLink;
        [SerializeField] private LinkTraversalEnum linkTraversalKind;
        [SerializeField] private Openable openable;

        public LinkTraversalEnum LinkTraversalKind => linkTraversalKind;
        public Openable Openable => openable;
        public NavMeshLink NavMeshLink => navMeshLink;
        private Coroutine reEnableLinkRoutine;
        private static readonly Collider[] results = new Collider[5];
        private const int LayerMask = 1 << 15;
        private const float MidpointOverlapRadius = 1f;

        private void Awake()
        {
            Openable.OnUnlocked.AddListener(EnableLink);
        }

        private void OnDestroy()
        {
            Openable.OnUnlocked.RemoveListener(EnableLink);
        }

        /// <summary>
        /// Disables the link for an amount of time equal to linkDisableTime.
        /// </summary>
        /// <param name="linkDisableTime"></param>
        public void TempDisableLink(float linkDisableTime)
        {
            NavMeshLink.enabled = false;
            if (reEnableLinkRoutine is not null) 
                StopCoroutine(reEnableLinkRoutine);
            reEnableLinkRoutine = StartCoroutine(ReEnableLinkRoutine(linkDisableTime));
        }
        
        private IEnumerator ReEnableLinkRoutine(float linkDisableTime)
        {
            yield return new WaitForSeconds(linkDisableTime);
            NavMeshLink.enabled = true;
            reEnableLinkRoutine = null;
        }

        private void EnableLink()
        {
            if (reEnableLinkRoutine is not null)
                StopCoroutine(reEnableLinkRoutine);
            NavMeshLink.enabled = true;
        }

        /// <summary>
        /// Used during runtime to retrieve the a link controller from the start and end points of a nav mesh link.  
        /// </summary>
        /// <param name="startPoint">Start point of the navMeshLink</param>
        /// <param name="endPoint">End point of the navMeshLink</param>
        /// <returns>The first link controller overlapped</returns>
        public static LinkController GetLinkFromEndPoints(Vector3 startPoint, Vector3 endPoint)
        {
            Vector3 midPoint = (startPoint + endPoint) / 2f;
            Debug.DrawLine(midPoint, midPoint + Vector3.up, Color.cyan, 1f);
            int numberOfColliders = Physics.OverlapSphereNonAlloc(midPoint, MidpointOverlapRadius, results, LayerMask, QueryTriggerInteraction.Collide);
            for (int i = 0; i < numberOfColliders; i++)
            {
                Collider overlappedCollider = results[i];
                LinkController link = overlappedCollider.GetComponentInParent<LinkController>();
                if (link)
                    return link;
            }

            return null;
        }
    }
}