using UnityEngine;

namespace Units
{
    public class UnitPlacer : MonoBehaviour
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
        
        public void CreateUnit(GameObject unitPrefab)
        {
            var newUnit = Instantiate(unitPrefab, instantiatePoint.position, instantiatePoint.rotation);
            
            newUnit.GetComponent<Unit>().OnClickOnGround(collectionPoint.position);
        }
    }
}