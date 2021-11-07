using Menu;
using Resources;
using UnityEngine;

namespace BuildingBase
{
    public enum BuildingState
    {
        Placed,
        Other
    }
    
    [RequireComponent(typeof(Price))]
    [RequireComponent(typeof(Health))]
    public class Building : SelectableObject
    {
        private BuildingPlacer _buildingPlacer;
        private Health _health;
        
        public Team team = Team.Neutral;

        public int xSize;
        public int zSize;

        public BuildingState currentState = BuildingState.Other;

        public Renderer itemRenderer;
        private Color _startColor;

        [SerializeField] private CraftMenu craftMenu;

        private void Awake()
        {
            _startColor = itemRenderer.material.color;
            _health = GetComponent<Health>();
        }

        public override void Start()
        {
            base.Start();
            
            Unselect();
        }

        private void OnDrawGizmos()
        {
            if (!_buildingPlacer)
            {
                _buildingPlacer = FindObjectOfType<BuildingPlacer>();
            }
            
            for (int x = 0; x < xSize; x++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    Gizmos.DrawWireCube(
                        transform.position + new Vector3(x, 0, z) * _buildingPlacer.cellSize,
                        new Vector3(1, 0, 1) * _buildingPlacer.cellSize
                    );
                }
            }
        }

        public void SetColor(Color color)
        {
            itemRenderer.material.color = color;
        }

        public void ResetColor()
        {
            itemRenderer.material.color = _startColor;
        }

        public override void Select()
        {
            base.Select();

            if (craftMenu)
            {
                craftMenu.gameObject.SetActive(true);
            }
        }

        public override void Unselect()
        {
            base.Unselect();

            if (craftMenu)
            {
                craftMenu.gameObject.SetActive(false);
            }
        }

        public void SetState(BuildingState state)
        {
            currentState = state;
        }

        public void TakeDamage(int damageValue)
        {
            _health.DecreaseHealth(damageValue);

            if (_health.healthSize <= 0)
            {
                Die();
            }
        }
        
        public void Die()
        {
            Debug.Log("unit die");
            Unselect();
            Destroy(gameObject);
        }
    }
}