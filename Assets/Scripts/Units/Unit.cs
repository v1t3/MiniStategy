using System;
using System.Security.Cryptography;
using BuildingBase;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    public enum UnitState
    {
        Idle,
        WalkToPoint,
        WalkToBuilding,
        WalkToEnemy,
        Attack
    }

    public class Unit : SelectableObject
    {
        private Management _management;

        public int price;

        [SerializeField] private HealthBar healthBar;
        public int health = 1;
        private int _maxHealth;

        public UnitState currentState;
        private Building _targetBuilding;
        private Enemy _targetEnemy;

        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private float distanceToFollow = 7;
        [SerializeField] private float distanceToAttack = 1;
        [SerializeField] private float attackPeriod = 1;
        private float _attackTimer;

        private Vector3 _targetPoint;
        private bool _isTargetReached;

        [SerializeField] private GameObject navigationIndicator;

        public override void Start()
        {
            base.Start();

            _management = FindObjectOfType<Management>();

            _maxHealth = health;
            healthBar.Setup();
            SetState(UnitState.Idle);
            navigationIndicator.SetActive(false);
            navigationIndicator.transform.parent = null;
        }

        private void Update()
        {
            if (currentState == UnitState.Idle)
            {
                FindClosestUnit();
            }
            else if (currentState == UnitState.WalkToPoint)
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
                        navigationIndicator.SetActive(false);
                        navMeshAgent.ResetPath();
                        SetState(UnitState.Idle);
                    }
                }
            }
            else if (currentState == UnitState.WalkToEnemy)
            {
                if (_targetEnemy)
                {
                    navMeshAgent.SetDestination(_targetEnemy.transform.position);

                    float distance = Vector3.Distance(transform.position, _targetEnemy.transform.position);

                    if (distance > distanceToFollow)
                    {
                        SetState(UnitState.Idle);
                    }

                    if (distance < distanceToAttack)
                    {
                        SetState(UnitState.Attack);
                    }
                }
                else
                {
                    SetState(UnitState.Idle);
                }
            }
            else if (currentState == UnitState.WalkToBuilding)
            {
                FindClosestUnit();

                if (!_targetBuilding)
                {
                    SetState(UnitState.Idle);
                }
            }
            else if (currentState == UnitState.Attack)
            {
                if (_targetEnemy)
                {
                    _isTargetReached = true;
                    navMeshAgent.ResetPath();

                    float distance = Vector3.Distance(transform.position, _targetEnemy.transform.position);

                    if (distance > distanceToAttack)
                    {
                        SetState(UnitState.WalkToEnemy);
                    }

                    _attackTimer += Time.deltaTime;

                    if (_attackTimer > attackPeriod)
                    {
                        //урон
                        _attackTimer = 0;
                        _targetEnemy.TakeDamage(1);
                    }
                }
                else
                {
                    SetState(UnitState.Idle);
                }
            }
        }

        public override void OnClickOnGround(Vector3 point)
        {
            base.OnClickOnGround(point);
            
            _targetPoint = point;
            navigationIndicator.SetActive(true);
            navigationIndicator.transform.position = new Vector3(
                point.x,
                navigationIndicator.transform.position.y,
                point.z
            );
            
            SetState(UnitState.WalkToPoint);
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
            _management.Unselect(this);
        }

        public void SetState(UnitState state)
        {
            currentState = state;

            if (currentState == UnitState.Idle)
            {
                // FindClosestBuilding();
            }
            else if (currentState == UnitState.WalkToPoint)
            {
                navMeshAgent.SetDestination(_targetPoint);
                _isTargetReached = false;
            }
            else if (currentState == UnitState.WalkToEnemy)
            {
                navMeshAgent.SetDestination(_targetEnemy.transform.position);
            }
            else if (currentState == UnitState.WalkToBuilding)
            {
                navMeshAgent.SetDestination(_targetBuilding.transform.position);
            }
            else if (currentState == UnitState.Attack)
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
                SetState(UnitState.WalkToBuilding);
            }
        }

        public void FindClosestUnit()
        {
            Enemy[] allEnemies = FindObjectsOfType<Enemy>();
            Enemy closestEnemy = null;
            float minDistance = Mathf.Infinity;

            foreach (var enemy in allEnemies)
            {
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