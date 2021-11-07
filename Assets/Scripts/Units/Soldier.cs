using UnityEngine;

namespace Units
{
    public class Soldier : Unit
    {
        [SerializeField] private GameObject navigationIndicator;
        
        public override void Start()
        {
            base.Start();

            if (navigationIndicator)
            {
                navigationIndicator.SetActive(false);
                navigationIndicator.transform.parent = null;
            }
        }
        
        public override void OnClickOnGround(Vector3 point)
        {
            base.OnClickOnGround(point);

            if (navigationIndicator)
            {
                navigationIndicator.SetActive(true);
                navigationIndicator.transform.position = new Vector3(
                    point.x,
                    navigationIndicator.transform.position.y,
                    point.z
                );
            }
        }

        private void OnDestroy()
        {
            if (navigationIndicator)
            {
                Destroy(navigationIndicator);
            }
        }
    }
}