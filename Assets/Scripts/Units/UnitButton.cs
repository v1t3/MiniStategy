using Player;
using UnityEngine;

namespace Units
{
    public class UnitButton : MonoBehaviour
    {
        private UnitPlacer _unitPlacer;
        
        public GameObject unitPrefab;

        private PlayerResources _playerResources;

        private void Start()
        {
            _unitPlacer = FindObjectOfType<UnitPlacer>();
            _playerResources = FindObjectOfType<PlayerResources>();
        }

        public void TryBuy()
        {
            int price = unitPrefab.GetComponent<Unit>().price;

            if (_playerResources.money >= price)
            {
                _playerResources.money -= price;
                _unitPlacer.CreateUnit(unitPrefab);
            }
            else
            {
                Debug.Log("enough money");
            }
        }
    }
}