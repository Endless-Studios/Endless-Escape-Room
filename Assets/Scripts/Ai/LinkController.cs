using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

namespace Ai
{
    public class LinkController : MonoBehaviour
    {
        [SerializeField] private NavMeshLink navMeshLink;
        [SerializeField] private LinkTraversalEnum linkTraversalKind;

        public LinkTraversalEnum LinkTraversalKind => linkTraversalKind;

        private Coroutine reEnableLinkRoutine;
        
        public void TempDisableLink(float linkDisableTime)
        {
            navMeshLink.enabled = false;
            if (reEnableLinkRoutine is not null) 
                StopCoroutine(reEnableLinkRoutine);
            reEnableLinkRoutine = StartCoroutine(ReEnableLinkRoutine(linkDisableTime));
        }

        private IEnumerator ReEnableLinkRoutine(float linkDisableTime)
        {
            yield return new WaitForSeconds(linkDisableTime);
            navMeshLink.enabled = true;
            reEnableLinkRoutine = null;
        }

        public void EnableLink()
        {
            if (reEnableLinkRoutine is not null)
                StopCoroutine(reEnableLinkRoutine);
            navMeshLink.enabled = true;
        }
    }
}