using System;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace Units
{
    public class UnitButton : MonoBehaviour
    {
        private PlayerResources _playerResources;
        private UnitPlacer _unitPlacer;

        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private float createUnitDelay;
        [SerializeField] private Image loadingImage;
        [SerializeField] private Text unitsToCreateText;

        private float _unitsToCreate;
        private float _createTimer;

        private void Start()
        {
            _unitPlacer = FindObjectOfType<UnitPlacer>();
            _playerResources = FindObjectOfType<PlayerResources>();
        }

        private void Update()
        {
            if (_unitsToCreate > 0)
            {
                _createTimer += Time.deltaTime;
                // Отображение процесса создания
                loadingImage.fillAmount = _createTimer / createUnitDelay;

                if (_createTimer > createUnitDelay)
                {
                    _createTimer = 0;
                    loadingImage.fillAmount = 0;
                    TryBuy();
                }
            }
        }

        private void TryBuy()
        {
            int price = unitPrefab.GetComponent<Unit>().price;

            if (_playerResources.money >= price)
            {
                _playerResources.money -= price;
                RemoveUnitToCreate();
                _unitPlacer.CreateUnit(unitPrefab);
            }
            else
            {
                Debug.Log("not enough money");
                ResetUnitToCreate();
            }
        }

        public void AddUnitToCreate()
        {
            _unitsToCreate++;
            ShowUnitsToCreateText();
        }

        public void RemoveUnitToCreate()
        {
            if (_unitsToCreate > 0)
            {
                _unitsToCreate--;
            }

            ShowUnitsToCreateText();
        }

        public void ResetUnitToCreate()
        {
            _unitsToCreate = 0;
            ShowUnitsToCreateText();
        }

        private void ShowUnitsToCreateText()
        {
            unitsToCreateText.text = _unitsToCreate > 0 ? _unitsToCreate.ToString() : "";
        }
    }
}