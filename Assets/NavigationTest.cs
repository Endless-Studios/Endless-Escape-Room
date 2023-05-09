using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationTest : MonoBehaviour
{
    public NavMeshAgent Agent;

    [ContextMenu("Move To Target")]
    public void MoveToTarget()
    {
        Agent.SetDestination(transform.position);
    }
}
