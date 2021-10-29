using UnityEngine;
using UnityEngine.AI;

public class Unit : SelectableObject
{
    [SerializeField] private NavMeshAgent navMeshAgent;

    public override void OnClickOnGround(Vector3 point)
    {
        base.OnClickOnGround(point);
        navMeshAgent.SetDestination(point);
        Debug.Log("point " + point);
    }
}