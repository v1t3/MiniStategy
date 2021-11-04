using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    public class Unit : SelectableObject
    {
        private Management _management;
        
        [SerializeField] private NavMeshAgent navMeshAgent;

        public int price;
        public int health = 1;
        private int _maxHealth;

        private bool _isTargetReached;

        [SerializeField] private HealthBar healthBar;

        public override void Start()
        {
            base.Start();
            
            _management = FindObjectOfType<Management>();
            
            _maxHealth = health;
            healthBar.Setup();
        }

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
            healthBar.SetHealth(health, _maxHealth);

            if (health <= 0)
            {
                health = 0;
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            Debug.Log("unit die");
            _management.UnselectAll();
        }
    }
}