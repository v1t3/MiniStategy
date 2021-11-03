using System.Collections.Generic;
using System.Security.Cryptography;
using Player;
using UnityEngine;

namespace BuildingBase
{
    public class BuildingPlacer : MonoBehaviour
    {
        private Management _management;
        private PlayerResources _playerResources;

        public float cellSize = 1f;

        [SerializeField] private Camera raycastCamera;
        private Plane _plane;

        public Building currentBuilding;

        public Dictionary<Vector2Int, Building> buildingPositions = new Dictionary<Vector2Int, Building>();

        private void Start()
        {
            _management = FindObjectOfType<Management>();
            _playerResources = FindObjectOfType<PlayerResources>();
            
            _plane = new Plane(Vector3.up, Vector3.zero);
            InstallExistedBuildings();
        }

        private void Update()
        {
            if (!currentBuilding) return;

            Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
            float distance;
            _plane.Raycast(ray, out distance);
            Vector3 point = ray.GetPoint(distance) / cellSize;

            int xPosition = Mathf.RoundToInt(point.x);
            int zPosition = Mathf.RoundToInt(point.z);

            currentBuilding.transform.position = new Vector3(xPosition, 0, zPosition) * cellSize;

            if (CanInstallBuilding(xPosition, zPosition, currentBuilding))
            {
                currentBuilding.DisplayAcceptablePosition();

                if (Input.GetMouseButtonDown(0))
                {
                    InstallBuilding(xPosition, zPosition, currentBuilding);
                }
                
            }
            else
            {
                currentBuilding.DisplayUnacceptablePosition();
            }
            
            // отменить установку по ПКМ или Escape
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelInstallBuilding();
            }
        }

        private bool CanInstallBuilding(int xPosition, int zPosition, Building building)
        {
            for (var x = 0; x < building.xSize; x++)
            {
                for (var z = 0; z < building.zSize; z++)
                {
                    if (buildingPositions.ContainsKey(new Vector2Int(xPosition + x, zPosition + z)))
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
                    buildingPositions.Add(new Vector2Int(xPosition + x, zPosition + z), building);
                }
            }

            currentBuilding = null;
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
            int price = currentBuilding.GetComponent<Building>().price;
            _playerResources.money += price;
            
            Destroy(currentBuilding.gameObject);
            currentBuilding = null;
            _management.currentBuildState = BuildState.Other;
        }

        public void CreateBuilding(GameObject buildingPrefab)
        {
            GameObject newBuilding = Instantiate(buildingPrefab);
            currentBuilding = newBuilding.GetComponent<Building>();
            _management.currentBuildState = BuildState.Installing;
        }
    }
}