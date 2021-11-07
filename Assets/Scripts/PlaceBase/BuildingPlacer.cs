using System.Collections.Generic;
using BuildingBase;
using Player;
using Resources;
using UnityEngine;

namespace PlaceBase
{
    public class BuildingPlacer : Placer
    {
        private Management _management;
        private PlayerResources _playerResources;

        public float cellSize = 1f;

        [SerializeField] private Camera raycastCamera;
        private Plane _plane;
        private Dictionary<Vector2Int, Building> _buildingPositions = new Dictionary<Vector2Int, Building>();
        private Building _currentBuilding;

        private void Start()
        {
            _management = FindObjectOfType<Management>();
            _playerResources = FindObjectOfType<PlayerResources>();
            
            _plane = new Plane(Vector3.up, Vector3.zero);
            InstallExistedBuildings();
        }

        private void Update()
        {
            if (!_currentBuilding) return;

            Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
            float distance;
            _plane.Raycast(ray, out distance);
            Vector3 point = ray.GetPoint(distance) / cellSize;

            int xPosition = Mathf.RoundToInt(point.x);
            int zPosition = Mathf.RoundToInt(point.z);

            _currentBuilding.transform.position = new Vector3(xPosition, 0, zPosition) * cellSize;

            if (CanInstallBuilding(xPosition, zPosition, _currentBuilding))
            {
                DisplayAcceptablePosition();

                if (Input.GetMouseButtonDown(0))
                {
                    InstallBuilding(xPosition, zPosition, _currentBuilding);
                }
                
            }
            else
            {
                DisplayUnacceptablePosition();
            }
            
            // отменить установку по ПКМ или Escape
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelInstallBuilding();
            }
        }

        public override void Create(GameObject buildingPrefab)
        {
            base.Create(buildingPrefab);
            
            GameObject newBuilding = Instantiate(buildingPrefab);
            _currentBuilding = newBuilding.GetComponent<Building>();
            _management.currentBuildState = BuildState.Installing;
        }

        public void DisplayUnacceptablePosition()
        {
            _currentBuilding.SetColor(Color.red);
        }

        public void DisplayAcceptablePosition()
        {
            _currentBuilding.ResetColor();
        }

        private bool CanInstallBuilding(int xPosition, int zPosition, Building building)
        {
            for (var x = 0; x < building.xSize; x++)
            {
                for (var z = 0; z < building.zSize; z++)
                {
                    if (_buildingPositions.ContainsKey(new Vector2Int(xPosition + x, zPosition + z)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void InstallBuilding(int xPosition, int zPosition, Building building)
        {
            for (var x = 0; x < building.xSize; x++)
            {
                for (var z = 0; z < building.zSize; z++)
                {
                    _buildingPositions.Add(new Vector2Int(xPosition + x, zPosition + z), building);
                }
            }

            building.SetState(BuildingState.Placed);
            _currentBuilding = null;
            _management.currentBuildState = BuildState.Other;
        }

        private void InstallExistedBuildings()
        {
            var buildings = FindObjectsOfType<Building>();

            foreach (var building in buildings)
            {
                var buildingPos = building.transform.position;
                InstallBuilding((int)buildingPos.x, (int)buildingPos.z, building);
            }
        }

        public void CancelInstallBuilding()
        {
            _playerResources.money += _currentBuilding.GetComponent<Price>().price;
            
            Destroy(_currentBuilding.gameObject);
            _currentBuilding = null;
            _management.currentBuildState = BuildState.Other;
        }
    }
}