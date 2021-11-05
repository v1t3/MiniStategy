using System;
using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyCreator : MonoBehaviour
{
    [SerializeField] private Transform spawn;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float creationPeriod;

    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > creationPeriod)
        {
            _timer = 0;
            Vector3 random = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

            var newEnemy = Instantiate(enemyPrefab, spawn.position, spawn.rotation);
            newEnemy.GetComponent<Enemy>().GoToPoint(spawn.position + random);
        }
    }
}
