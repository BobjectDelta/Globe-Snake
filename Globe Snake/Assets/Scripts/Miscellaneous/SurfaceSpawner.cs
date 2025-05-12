using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceSpawner : MonoBehaviour
{
    [SerializeField] private Transform _surfaceObjectTransform;
    [SerializeField] private List<Bonus> bonuses = new List<Bonus>();
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

            Bonus bonus = bonuses[Random.Range(0, bonuses.Count)];
            Collider bonusCollider = bonus.GetComponent<Collider>();
            float bonusCheckRadius = bonusCollider.bounds.size.magnitude * 5;

            Vector3 spawnPosition = Vector3.zero;
            bool canSpawn = false;
            int attempts = 0;

            while (!canSpawn && attempts < _maxSpawnAttempts)
            {
                attempts++;
                Vector3 randomSurfacePoint = Random.onUnitSphere * (_surfaceObjectRadius + _heightOffset * bonus.transform.lossyScale.z);
                spawnPosition = _surfaceObjectTransform.position + randomSurfacePoint;

                if (!Physics.CheckSphere(spawnPosition, bonusCheckRadius, _obstructionLayers))               
                    canSpawn = true;         
                else
                    Debug.Log("Spawn position is occupied, trying again");
            }

            if (canSpawn)
            {
                GameObject bonusPrefab = Instantiate(bonus.gameObject, spawnPosition, Quaternion.identity);
                bonusPrefab.transform.SetParent(_bonusHolder);
            }
            else           
                Debug.Log("Bonus not spawned, collision detected");

            _spawnTimer = Time.time + _spawnInterval;
                
        }
    }
}
