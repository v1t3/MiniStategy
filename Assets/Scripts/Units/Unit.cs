using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    public class Unit : SelectableObject
    {
        [SerializeField] private NavMeshAgent navMeshAgent;

        public int price;
        public int health = 1;

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

        public void TakeDamage(int damageValue)
        {
            health -= damageValue;

            if (health <= 0)
            {
                health = 0;
                Debug.Log("unit die");
            }
        }
    }
}