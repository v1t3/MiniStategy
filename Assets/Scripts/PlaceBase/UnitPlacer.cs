using Units;
using UnityEngine;

namespace PlaceBase
{
    public class UnitPlacer : Placer
    {
        public Transform collectionPoint;
        public Transform instantiatePoint;

        private void Start()
        {
            HideCollectionPoint();
        }

        public void ShowCollectionPoint()
        {
            collectionPoint.gameObject.SetActive(true);
        }

        public void HideCollectionPoint()
        {
            collectionPoint.gameObject.SetActive(false);
        }
        
        public override void Create(GameObject unitPrefab)
        {
            base.Create(unitPrefab);

            var newUnit = Instantiate(unitPrefab, instantiatePoint.position, instantiatePoint.rotation);
            
            newUnit.GetComponent<Unit>().OnClickOnGround(collectionPoint.position);
        }
    }
}