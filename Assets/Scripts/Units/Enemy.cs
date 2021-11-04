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
        WalkToBuilding,
        WalkToUnit,
        Attack
    }

    public class Enemy : MonoBehaviour
    {
        public EnemyState currentEnemyState;
        private Building _targetBuilding;
        private Unit _targetUnit;

        [SerializeField] private HealthBar healthBar;
        [SerializeField] private float health = 1;
        [SerializeField] private float distanceToFollow = 7;
        [SerializeField] private float distanceToAttack = 1;

        public NavMeshAgent navMeshAgent;

        [SerializeField] private float attackPeriod = 1;
        private float _attackTimer;

        private void Start()
        {
            SetState(EnemyState.Idle);
        }

        private void Update()
        {
            if (currentEnemyState == EnemyState.Idle)
            {
                FindClosestUnit();
            }
            else if (currentEnemyState == EnemyState.WalkToUnit)
            {
                if (_targetUnit)
                {
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
                    float distance = Vector3.Distance(transform.position, _targetUnit.transform.position);

                    if (distance > distanceToAttack)
                    {
                        SetState(EnemyState.WalkToUnit);
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
            else if (currentEnemyState == EnemyState.WalkToUnit)
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
                SetState(EnemyState.WalkToUnit);
            }
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
    }
}