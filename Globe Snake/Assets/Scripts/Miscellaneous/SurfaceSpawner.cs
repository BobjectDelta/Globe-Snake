using System.Collections.Generic;
using UnityEngine;

public class SurfaceSpawner : MonoBehaviour
{
    [SerializeField] private Transform _surfaceObjectTransform;
    [SerializeField] private List<GameObject> bonuses = new List<GameObject>();
    [SerializeField] private Transform _bonusHolder;
    [SerializeField] private float _spawnInterval = 3f;
    [SerializeField] private float _heightOffset = 0.5f;
    [SerializeField] private int _maxActiveBonuses = 10;
    [SerializeField] private LayerMask _obstructionLayers;

    private int _maxSpawnAttempts = 5;
    private float _spawnTimer = 0f;
    private float _surfaceObjectRadius;

    private void Start()
    {
        _surfaceObjectRadius = _surfaceObjectTransform.lossyScale.x / 2f;
        _spawnTimer = Time.time + _spawnInterval;
    }

    private void Update()
    {
        SpawnBonus();
    }

    private void SpawnBonus()
    {
        if (bonuses.Count > 0 && Time.time > _spawnTimer)
        {
            if (_bonusHolder.childCount >= _maxActiveBonuses)
            {               
                _spawnTimer = Time.time + _spawnInterval; 
                return; 
            }

            GameObject bonusPrefab = bonuses[Random.Range(0, bonuses.Count)];
            float bonusCheckRadius = bonusPrefab.transform.lossyScale.x * 2;

            Vector3 spawnPosition = Vector3.zero;
            bool canSpawn = false;
            int attempts = 0;

            while (!canSpawn && attempts < _maxSpawnAttempts)
            {
                attempts++;
                Vector3 randomSurfacePoint = Random.onUnitSphere * (_surfaceObjectRadius + _heightOffset * bonusPrefab.transform.lossyScale.z);
                spawnPosition = _surfaceObjectTransform.position + randomSurfacePoint;
                if (!Physics.CheckSphere(spawnPosition, bonusCheckRadius, _obstructionLayers))               
                    canSpawn = true;         
                else
                    Debug.Log("Spawn position is occupied, trying again");
            }

            if (canSpawn)
            {
                Vector3 normal = (spawnPosition - _surfaceObjectTransform.position).normalized;
                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, normal);
                GameObject bonus = Instantiate(bonusPrefab, spawnPosition, spawnRotation);
                bonus.transform.SetParent(_bonusHolder);
            }
            else           
                Debug.Log("Bonus not spawned, collision detected");

            _spawnTimer = Time.time + _spawnInterval;               
        }
    }
}
