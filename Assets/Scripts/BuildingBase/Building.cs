using Menu;
using UnityEngine;

namespace BuildingBase
{
    public enum BuildingState
    {
        Placed,
        Other
    }
    
    public class Building : SelectableObject
    {
        public int price;
        public int xSize;
        public int zSize;

        private BuildingPlacer _buildingPlacer;
        public BuildingState currentState = BuildingState.Other;

        private Color _startColor;
        public Renderer itemRenderer;

        [SerializeField] private CraftMenu craftMenu;

        private void Awake()
        {
            _startColor = itemRenderer.material.color;
        }

        private void Start()
        {
            Unselect();
        }

        private void OnDrawGizmos()
        {
            _buildingPlacer = FindObjectOfType<BuildingPlacer>();
            
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

        public void DisplayUnacceptablePosition()
        {
            itemRenderer.material.color = Color.red;
        }

        public void DisplayAcceptablePosition()
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
    }
}