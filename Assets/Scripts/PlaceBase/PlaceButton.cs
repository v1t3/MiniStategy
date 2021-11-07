using Player;
using Resources;
using UnityEngine;
using UnityEngine.UI;

namespace PlaceBase
{
    public class PlaceButton : MonoBehaviour
    {
        private Placer _placer;
        
        private PlayerResources _playerResources;
        
        public GameObject objectPrefab;
        private Price _objectPrice;

        [SerializeField] private float createDelay;
        [SerializeField] private Image loadingImage;
        [SerializeField] private Text leftToCreateText;
        [SerializeField] private Text priceText;

        private int _leftToCreate;
        private float _createTimer;

        public virtual void Start()
        {
            _playerResources = FindObjectOfType<PlayerResources>();
            _objectPrice = objectPrefab.GetComponent<Price>();
            ShowPriceText();
        }

        private void Update()
        {
            if (_leftToCreate > 0)
            {
                _createTimer += Time.deltaTime;
                // Отображение процесса создания
                loadingImage.fillAmount = _createTimer / createDelay;

                if (_createTimer > createDelay)
                {
                    _createTimer = 0;
                    loadingImage.fillAmount = 0;
                    TryBuy();
                }
            }
        }

        private void TryBuy()
        {
            if (_objectPrice)
            {
                if (_playerResources.Money > 0 && _playerResources.Money >= _objectPrice.price)
                {
                    _playerResources.Money -= _objectPrice.price;
                    RemoveLeftToCreate();
                    _placer.Create(objectPrefab);
                }
                else
                {
                    Debug.Log("not enough money");
                    ResetLeftToCreate();
                }
            }
            else
            {
                Debug.Log("no price");
                ResetLeftToCreate();
            }
        }

        public void SetPlacer(Placer placerItem)
        {
            _placer = placerItem;
        }

        public void AddUnitToCreate()
        {
            if (_playerResources.Money > 0 && _playerResources.Money >= _objectPrice.price)
            {
                _leftToCreate++;
                ShowLeftToCreateText();
            }
            else
            {
                Debug.Log("not enough money");
                ResetLeftToCreate();
            }
        }

        private void RemoveLeftToCreate()
        {
            if (_leftToCreate > 0)
            {
                _leftToCreate--;
            }

            ShowLeftToCreateText();
        }

        private void ResetLeftToCreate()
        {
            _leftToCreate = 0;
            _createTimer = 0;
            loadingImage.fillAmount = 0;
            ShowLeftToCreateText();
        }

        private void ShowLeftToCreateText()
        {
            leftToCreateText.text = _leftToCreate > 0 ? _leftToCreate.ToString() : "";
        }

        private void ShowPriceText()
        {
            priceText.text = _objectPrice ? _objectPrice.price.ToString() : "";
        }
    }
}