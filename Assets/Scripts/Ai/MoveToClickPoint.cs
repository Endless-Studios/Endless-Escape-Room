using UnityEngine;
using UnityEngine.AI;
    
/// <summary>
/// Test script. Do not use for gameplay.
/// </summary>
public class MoveToClickPoint : MonoBehaviour 
{
    [SerializeField] private NavMeshAgent agent;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100)) 
            {
                agent.destination = hit.point;
            }
        }
    }
}