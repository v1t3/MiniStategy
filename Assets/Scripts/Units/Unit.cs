using System;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    public class Unit : SelectableObject
    {
        [SerializeField] private NavMeshAgent navMeshAgent;

        public int price;

        private bool _isTargetReached;

        private void Update()
        {
            if (
                !_isTargetReached &&
                !navMeshAgent.pathPending &&
                navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance
            )
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    _isTargetReached = true;
                    navMeshAgent.ResetPath();
                }
            }
        }

        public override void OnClickOnGround(Vector3 point)
        {
            base.OnClickOnGround(point);
            navMeshAgent.SetDestination(point);
            _isTargetReached = false;
        }
    }
}