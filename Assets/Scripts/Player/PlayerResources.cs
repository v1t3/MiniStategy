using System;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerResources : MonoBehaviour
    {
        [SerializeField] private int money;
        [SerializeField] private Text moneyText;

        public int Money
        {
            get { return money; }
            set
            {
                money = value;
                UpdateMoneyText();
            }
        }

        private void Start()
        {
            UpdateMoneyText();
        }

        private void UpdateMoneyText()
        {
            moneyText.text = money.ToString();
        }
    }
}