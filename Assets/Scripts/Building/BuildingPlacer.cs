using System.Collections.Generic;
using UnityEngine;

namespace Building
{
    public class BuildingPlacer : MonoBehaviour
    {
        public float cellSize = 1f;

        [SerializeField] private Camera raycastCamera;
        private Plane _plane;

        public Building currentBuilding;

        public Dictionary<Vector2Int, Building> buildingPositions = new Dictionary<Vector2Int, Building>();

        private void Start()
        {
            _plane = new Plane(Vector3.up, Vector3.zero);
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
                    currentBuilding = null;
                }
            }
            else
            {
                currentBuilding.DisplayUnacceptablePosition();
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
        }

        public void CreateBuilding(GameObject buildingPrefab)
        {
            GameObject newBuilding = Instantiate(buildingPrefab);
            currentBuilding = newBuilding.GetComponent<Building>();
        }
    }
}