using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private Openable openable;
        [SerializeField] private List<LinkInfo> links = new List<LinkInfo>();
        
        public Openable Openable => openable;
        
        private Coroutine reEnableLinkRoutine;

        private void Awake()
        {
            if(Openable)
                Openable.OnUnlocked.AddListener(EnableLink);
        }

        /// <summary>
        /// Disables the link for an amount of time equal to linkDisableTime.
        /// </summary>
        /// <param name="agentType"></param>
        /// <param name="linkDisableTime"></param>
        public void DisableLink(int agentType, float linkDisableTime)
        {
            GetLinkForAgentType(agentType).enabled = false;
            if (reEnableLinkRoutine is not null) 
                StopCoroutine(reEnableLinkRoutine);
            reEnableLinkRoutine = StartCoroutine(ReenableLinkRoutine(agentType, linkDisableTime));
        }
        
        private IEnumerator ReenableLinkRoutine(int agentTypeId, float linkDisableTime)
        {
            yield return new WaitForSeconds(linkDisableTime);
            GetLinkForAgentType(agentTypeId).enabled = true;
            reEnableLinkRoutine = null;
        }

        private void EnableLink()
        {
            if (reEnableLinkRoutine is not null)
                StopCoroutine(reEnableLinkRoutine);
            
            foreach (LinkInfo linkInfo in links)
            {
                linkInfo.Link.enabled = true;
            }
        }

        private NavMeshLink GetLinkForAgentType(int agentTypeId)
        {
            foreach (LinkInfo linkInfo in links)
            {
                if (linkInfo.Link.agentTypeID == agentTypeId)
                    return linkInfo.Link;
            }

            throw new Exception("No appropriate Nav Mesh Link found");
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
        
        private static readonly Collider[] results = new Collider[5];
        private const int LayerMask = 1 << 15;
        private const float MidpointOverlapRadius = 1f;
    }
}