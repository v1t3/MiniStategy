using BuildingBase;
using PlaceBase;
using UnityEngine;

public class EnemyCreator : MonoBehaviour
{
    [SerializeField] private Barack barack;
    private UnitPlacer _unitPlacer;
    
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float creationPeriod;

    private float _timer;

    private void Start()
    {
        _unitPlacer = barack.GetComponent<UnitPlacer>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > creationPeriod)
        {
            _timer = 0;
            _unitPlacer.Create(enemyPrefab);
        }
    }
}
