using System;
using BuildingBase;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    public enum EnemyState
    {
        Idle,
        WalkToPoint,
        WalkToBuilding,
        WalkToEnemy,
        Attack
    }

    public class Enemy : MonoBehaviour
    {
        public EnemyState currentEnemyState;
        private Building _targetBuilding;
        private Unit _targetUnit;

        [SerializeField] private HealthBar healthBar;
        [SerializeField] private int health = 1;
        private int _maxHealth;
        
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private float distanceToFollow = 7;
        [SerializeField] private float distanceToAttack = 1;
        [SerializeField] private float attackPeriod = 1;
        private float _attackTimer;

        private Vector3 _targetPoint;
        private bool _isTargetReached;

        private void Start()
        {
            SetState(EnemyState.Idle);
            _maxHealth = health;
        }

        private void Update()
        {
            if (currentEnemyState == EnemyState.Idle)
            {
                FindClosestBuilding();
                FindClosestUnit();
            }
            else if (currentEnemyState == EnemyState.WalkToPoint)
            {
                FindClosestUnit();

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
                        SetState(EnemyState.Idle);
                    }
                }
            }
            else if (currentEnemyState == EnemyState.WalkToEnemy)
            {
                if (_targetUnit)
                {
                    navMeshAgent.SetDestination(_targetUnit.transform.position);

                    float distance = Vector3.Distance(transform.position, _targetUnit.transform.position);

                    if (distance > distanceToFollow)
                    {
                        SetState(EnemyState.Idle);
                    }

                    if (distance < distanceToAttack)
                    {
                        SetState(EnemyState.Attack);
                    }
                }
                else
                {
                    SetState(EnemyState.Idle);
                }
            }
            else if (currentEnemyState == EnemyState.WalkToBuilding)
            {
                FindClosestUnit();

                if (!_targetBuilding)
                {
                    SetState(EnemyState.Idle);
                }
            }
            else if (currentEnemyState == EnemyState.Attack)
            {
                if (_targetUnit)
                {
                    navMeshAgent.ResetPath();
                    
                    float distance = Vector3.Distance(transform.position, _targetUnit.transform.position);

                    if (distance > distanceToAttack)
                    {
                        SetState(EnemyState.WalkToEnemy);
                    }

                    _attackTimer += Time.deltaTime;

                    if (_attackTimer > attackPeriod)
                    {
                        //урон
                        _attackTimer = 0;
                        _targetUnit.TakeDamage(1);
                    }
                }
                else
                {
                    SetState(EnemyState.Idle);
                }
            }
        }

        public void SetState(EnemyState state)
        {
            currentEnemyState = state;

            if (currentEnemyState == EnemyState.Idle)
            {
                FindClosestBuilding();
            }
            else if (currentEnemyState == EnemyState.WalkToPoint)
            {
                navMeshAgent.SetDestination(_targetPoint);
                _isTargetReached = false;
            }
            else if (currentEnemyState == EnemyState.WalkToEnemy)
            {
                navMeshAgent.SetDestination(_targetUnit.transform.position);
            }
            else if (currentEnemyState == EnemyState.WalkToBuilding)
            {
                navMeshAgent.SetDestination(_targetBuilding.transform.position);
            }
            else if (currentEnemyState == EnemyState.Attack)
            {
                _attackTimer = 0;
            }
        }

        public void FindClosestBuilding()
        {
            Building[] allBuildings = FindObjectsOfType<Building>();
            Building closestBuilding = null;
            float minDistance = Mathf.Infinity;

            foreach (var building in allBuildings)
            {
                if (building.currentState != BuildingState.Placed)
                {
                    continue;
                }
                
                float distance = Vector3.Distance(transform.position, building.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestBuilding = building;
                }
            }

            _targetBuilding = closestBuilding;

            if (_targetBuilding)
            {
                SetState(EnemyState.WalkToBuilding);
            }
        }

        public void FindClosestUnit()
        {
            Unit[] allUnit = FindObjectsOfType<Unit>();
            Unit closestUnit = null;
            float minDistance = Mathf.Infinity;

            foreach (var unit in allUnit)
            {
                float distance = Vector3.Distance(transform.position, unit.transform.position);

                if (distance < minDistance && distance < distanceToFollow)
                {
                    minDistance = distance;
                    closestUnit = unit;
                }
            }

            _targetUnit = closestUnit;

            if (_targetUnit)
            {
                SetState(EnemyState.WalkToEnemy);
            }
        }

        public void GoToPoint(Vector3 point)
        {
            _targetPoint = point;
            SetState(EnemyState.WalkToPoint);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(transform.position, Vector3.up, distanceToFollow);
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, Vector3.up, distanceToAttack);
        }
#endif

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
            Debug.Log("enemy die");
        }
    }
}