using System;
using BuildingBase;
using PlaceBase;
using Resources;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Units
{
    public enum UnitState
    {
        Idle,
        WalkToPoint,
        WalkToBuilding,
        WalkToEnemy,
        AttackBuilding,
        AttackUnit
    }

    [RequireComponent(typeof(Price))]
    [RequireComponent(typeof(Health))]
    public class Unit : SelectableObject
    {
        private Management _management;
        private BuildingPlacer _buildingPlacer;

        private Health _health;
        private Price _price;

        public Team team = Team.Neutral;

        public UnitState currentState;
        private Building _targetBuilding;
        private Vector3 _targetBuildingPoint;
        private Unit _targetEnemy;

        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private float distanceToFollow = 7;
        [SerializeField] private float distanceToAttack = 1;
        [SerializeField] private float attackPeriod = 1;
        private float _attackTimer;

        private Vector3 _targetPoint;
        private bool _isTargetReached;
        private Vector3 _walkToPointTimer;

        [SerializeField] private float maxFindPeriod = 3;
        private float _findTimer;

        [Space] [SerializeField] private UnityEvent onTargetPointReached;

        public override void Start()
        {
            base.Start();

            _management = FindObjectOfType<Management>();
            _health = GetComponent<Health>();
            _price = GetComponent<Price>();
            _buildingPlacer = FindObjectOfType<BuildingPlacer>();

            SetState(UnitState.Idle);
        }

        private void Update()
        {
            switch (currentState)
            {
                case UnitState.Idle:
                {
                    FindClosestEnemyBuilding();
                    FindClosestEnemy();

                    break;
                }
                case UnitState.WalkToPoint:
                {
                    _findTimer += Time.deltaTime;

                    if (_findTimer > maxFindPeriod)
                    {
                        _findTimer = 0;
                        FindClosestEnemyBuilding();
                        FindClosestEnemy();
                    }

                    if (IsTargetPointReached())
                    {
                        ResetTargetPoint();
                        SetState(UnitState.Idle);
                    }

                    break;
                }
                case UnitState.WalkToEnemy:
                {
                    if (!_targetEnemy)
                    {
                        SetState(UnitState.Idle);
                        return;
                    }

                    var enemyPosition = _targetEnemy.transform.position;
                    navMeshAgent.SetDestination(enemyPosition);
                    float distance = Vector3.Distance(transform.position, enemyPosition);

                    if (distance > distanceToFollow)
                    {
                        SetState(UnitState.Idle);
                        return;
                    }

                    if (distance < distanceToAttack)
                    {
                        SetState(UnitState.AttackUnit);
                    }

                    break;
                }
                case UnitState.WalkToBuilding:
                {
                    FindClosestEnemy();

                    if (!_targetBuilding)
                    {
                        SetState(UnitState.Idle);
                    }

                    navMeshAgent.SetDestination(_targetBuildingPoint);
                    var distance = Vector3.Distance(transform.position, _targetBuildingPoint);

                    if (distance > distanceToFollow)
                    {
                        SetState(UnitState.Idle);
                        return;
                    }

                    if (distance < distanceToAttack)
                    {
                        SetState(UnitState.AttackBuilding);
                    }

                    break;
                }
                case UnitState.AttackBuilding:
                {
                    if (!_targetBuilding)
                    {
                        SetState(UnitState.Idle);
                        return;
                    }

                    ResetTargetPoint();

                    var distance = Vector3.Distance(transform.position, _targetBuildingPoint);

                    if (distance > distanceToAttack)
                    {
                        SetState(UnitState.WalkToBuilding);
                        return;
                    }

                    _attackTimer += Time.deltaTime;

                    if (_attackTimer > attackPeriod)
                    {
                        //урон
                        _attackTimer = 0;
                        AttackBuilding();
                    }

                    break;
                }
                case UnitState.AttackUnit:
                {
                    if (!_targetEnemy)
                    {
                        SetState(UnitState.Idle);
                        return;
                    }

                    ResetTargetPoint();

                    var enemyPosition = _targetEnemy.transform.position;
                    float distance = Vector3.Distance(transform.position, enemyPosition);

                    if (distance > distanceToAttack)
                    {
                        SetState(UnitState.WalkToEnemy);
                        return;
                    }

                    _attackTimer += Time.deltaTime;

                    if (_attackTimer > attackPeriod)
                    {
                        //урон
                        _attackTimer = 0;
                        AttackUnit();
                    }

                    break;
                }
            }
        }

        public virtual void AttackBuilding()
        {
            _targetBuilding.TakeDamage(1);
        }

        public virtual void AttackUnit()
        {
            _targetEnemy.TakeDamage(1);
        }

        private bool IsTargetPointReached()
        {
            if (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                return false;
            }

            return !navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f;
        }

        private void ResetTargetPoint()
        {
            navMeshAgent.ResetPath();
            onTargetPointReached.Invoke();
        }

        private void SetState(UnitState state)
        {
            currentState = state;

            switch (currentState)
            {
                case UnitState.Idle:
                {
                    break;
                }
                case UnitState.WalkToPoint:
                {
                    if (_targetPoint != Vector3.zero)
                    {
                        _findTimer = 0;
                        navMeshAgent.SetDestination(_targetPoint);
                    }

                    break;
                }
                case UnitState.WalkToEnemy:
                {
                    if (_targetEnemy)
                    {
                        navMeshAgent.SetDestination(_targetEnemy.transform.position);
                    }

                    break;
                }
                case UnitState.WalkToBuilding:
                {
                    if (_targetBuilding)
                    {
                        navMeshAgent.SetDestination(_targetBuildingPoint);
                    }

                    break;
                }
            }
        }

        public override void OnClickOnGround(Vector3 point)
        {
            base.OnClickOnGround(point);
            GoToPoint(point);
        }

        public void GoToPoint(Vector3 point)
        {
            _targetPoint = point;
            SetState(UnitState.WalkToPoint);
        }

        private void FindClosestEnemyBuilding()
        {
            Building[] allBuildings = FindObjectsOfType<Building>(false);
            Building closestBuilding = null;
            Vector3 closestBuildingPoint = Vector3.zero;
            float minDistance = Mathf.Infinity;

            foreach (var building in allBuildings)
            {
                if (
                    building.currentState != BuildingState.Placed ||
                    building.team == team ||
                    building.team == Team.Neutral
                )
                {
                    continue;
                }

                var xSize = building.xSize;
                var zSize = building.zSize;

                for (int x = 0; x < xSize; x++)
                {
                    for (int z = 0; z < zSize; z++)
                    {
                        var position = building.transform.position + new Vector3(x, 0, z);
                        float distance = Vector3.Distance(transform.position, position);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestBuilding = building;
                            closestBuildingPoint = position;
                        }
                    }
                }
            }

            _targetBuilding = closestBuilding;
            _targetBuildingPoint = closestBuildingPoint;

            if (_targetBuilding)
            {
                SetState(UnitState.WalkToBuilding);
            }
        }

        private void FindClosestEnemy()
        {
            Unit[] allEnemies = FindObjectsOfType<Unit>();
            Unit closestEnemy = null;
            float minDistance = Mathf.Infinity;

            foreach (var enemy in allEnemies)
            {
                if (enemy.team == team || enemy.team == Team.Neutral)
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < minDistance && distance < distanceToFollow)
                {
                    minDistance = distance;
                    closestEnemy = enemy;
                }
            }

            _targetEnemy = closestEnemy;

            if (_targetEnemy)
            {
                SetState(UnitState.WalkToEnemy);
            }
        }

        public void TakeDamage(int damageValue)
        {
            _health.DecreaseHealth(damageValue);

            if (_health.healthSize <= 0)
            {
                Die();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var position = transform.position;
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(position, Vector3.up, distanceToFollow);
            Handles.color = Color.red;
            Handles.DrawWireDisc(position, Vector3.up, distanceToAttack);
        }
#endif

        public void Die()
        {
            Debug.Log("unit die");
            _management.Unselect(this);
            Destroy(gameObject);
        }
    }
}