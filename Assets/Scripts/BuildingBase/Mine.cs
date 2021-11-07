using Player;
using Resources;
using UnityEngine;
using UnityEngine.UI;

namespace BuildingBase
{
    public class Mine : Building
    {
        private PlayerResources _playerResources;

        [SerializeField] private CollectButton collectButton;
        
        [SerializeField] private int collectValue;
        private int _leftToCollect;
        
        [SerializeField] private float createDelay;
        private float _createTimer;

        public override void Start()
        {
            base.Start();

            if (team != Team.Player) return;
            
            _playerResources = FindObjectOfType<PlayerResources>();
            collectButton.UpdateCollectText(collectValue);
        }

        private void Update()
        {
            if (team != Team.Player) return;

            _createTimer += Time.deltaTime;
            
            // Отображение заполнения шкалы
            var fillAmount = _createTimer / createDelay;
            collectButton.UpdateAmount(fillAmount);

            if (_createTimer > createDelay)
            {
                _createTimer = 0;
                collectButton.UpdateAmount(0);
                CollectMoney();
            }
        }

        private void CollectMoney()
        {
            _playerResources.Money += collectValue;
        }
    }
}
