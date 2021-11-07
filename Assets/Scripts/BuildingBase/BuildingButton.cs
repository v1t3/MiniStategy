using Player;
using Resources;
using UnityEngine;

namespace BuildingBase
{
    public class BuildingButton : MonoBehaviour
    {
        public BuildingPlacer buildingPlacer;
        public GameObject buildingPrefab;

        private PlayerResources _playerResources;

        private void Start()
        {
            _playerResources = FindObjectOfType<PlayerResources>();
        }

        public void TryBuy()
        {
            int price = buildingPrefab.GetComponent<Price>().price;

            if (_playerResources.money >= price)
            {
                _playerResources.money -= price;
                buildingPlacer.CreateBuilding(buildingPrefab);
            }
            else
            {
                Debug.Log("enough money");
            }
        }
    }
}
