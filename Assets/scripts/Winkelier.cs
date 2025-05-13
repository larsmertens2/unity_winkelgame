using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Winkelier : MonoBehaviour
{
    public List<Transform> GoPoints;
    public NavMeshAgent navMeshAgent;

    private float stuckTimer = 0f;
    public float maxTravelTime = 5f;

    // Update is called once per frame
    void Update()
    {
        if (navMeshAgent.pathPending) return;

        // Increment stuck timer
        stuckTimer += Time.deltaTime;

        // Reached destination OR took too long
        if ((navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.hasPath) || stuckTimer >= maxTravelTime)
        {
            navMeshAgent.SetDestination(GoPoints[Random.Range(0, GoPoints.Count)].position);
            stuckTimer = 0f; // Reset timer after setting new destination
        }
    }
}
