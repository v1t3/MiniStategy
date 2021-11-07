using UnityEngine;

namespace Resources
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private HealthBar healthBar;
        public int healthSize = 1;
        private int _maxHealthSize;

        private void Start()
        {
            _maxHealthSize = healthSize;
        }

        public void IncreaseHealth(int value)
        {
            healthSize += value;

            if (healthSize > _maxHealthSize)
            {
                healthSize = _maxHealthSize;
            }
        
            healthBar.SetHealth(healthSize, _maxHealthSize);
        }

        public void DecreaseHealth(int value)
        {
            healthSize -= value;
        
            if (healthSize < 0)
            {
                healthSize = 0;
            }
        
            healthBar.SetHealth(healthSize, _maxHealthSize);
        }
    }
}