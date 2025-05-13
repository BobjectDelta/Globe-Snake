using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Transform _planetTransform; 
    [SerializeField] private List<GameObject> _obstaclePrefabs;
    [SerializeField] private Transform _generatedObjectsHolder; 

    [Header("Generation Settings")]
    [SerializeField] private LayerMask _obstructionLayers;
    [SerializeField] private int _obstacleCount = 50; 
    [SerializeField] private float _minSpawnPointSpacing = 10f; 
    [SerializeField] private int _maxPlacementAttempts = 10; 

    private float _planetRadius; 
    private Vector3 _planetCenter; 
    private int _totalPlacedObstacles = 0; 
    public Transform GeneratedPlayerSpawnPoint { get; private set; }

    void Awake()
    {
        if (_planetTransform == null)
            Debug.LogError("Planet Transform is not assigned in LevelGenerator!", this);
        if (_obstaclePrefabs == null || _obstaclePrefabs.Count == 0)
            Debug.LogError("No Obstacle Prefabs assigned in LevelGenerator!", this);

        _planetCenter = _planetTransform.position;
        _planetRadius = _planetTransform.lossyScale.x / 2f;
        _obstacleCount = Random.Range(50, 80);

        if (_generatedObjectsHolder == null)
        {
            GameObject parentGO = new GameObject("GeneratedLevelObjects");
            _generatedObjectsHolder = parentGO.transform;
        }

        GenerateLevel();
        SetLevelParameters(1, 5);
    }

    void GenerateLevel()
    {
        Debug.Log("Starting level generation...");

        PlacePlayerSpawnPoint();      
        if (_obstaclePrefabs != null && _obstaclePrefabs.Count > 0)       
            PlaceObstacles();
        
        Debug.Log($"Level generation finished. Placed {GetPlacedObstacleCount()} obstacles and 1 spawn point.");
    }

    private void PlacePlayerSpawnPoint()
    {
        Vector3 spawnPosition = GetPointOnSphereSurface(_planetRadius);
        Vector3 normal = (spawnPosition - _planetCenter).normalized; 
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, normal); 

        GameObject spawnPointGO = new GameObject("SpawnPoint"); 
        spawnPointGO.transform.position = spawnPosition;
        spawnPointGO.transform.rotation = spawnRotation;
        spawnPointGO.transform.SetParent(_generatedObjectsHolder); 

        GeneratedPlayerSpawnPoint = spawnPointGO.transform; 
        Debug.Log($"Placed Player Spawn Point at {spawnPosition}.");

        GameManager gameManager = GameManager.gameManagerInstance;
        if (gameManager != null)               
            gameManager.SetPlayerSpawnPoint(GeneratedPlayerSpawnPoint);        
        else        
            Debug.LogWarning("GameManager instance not found or null. Cannot assign player spawn point.", this); 
    }

    private void PlaceObstacles()
    {
        for (int i = 0; i < _obstacleCount; i++)
        {
            GameObject randomObstaclePrefab = _obstaclePrefabs[Random.Range(0, _obstaclePrefabs.Count)];
            bool successfullyPlaced = TryPlaceObject(randomObstaclePrefab);       
        }
    }

    private bool TryPlaceObject(GameObject objectPrefab)
    {
        Collider prefabCollider = objectPrefab.GetComponent<Collider>();
        float obstacleCheckRadius = 1f; 
        if (prefabCollider != null)       
            obstacleCheckRadius = objectPrefab.transform.lossyScale.magnitude / 2;      
        else       
            Debug.LogWarning($"Obstacle prefab '{objectPrefab.name}' is missing a Collider. Using default check radius ({obstacleCheckRadius}).", objectPrefab);

        int attempts = 0;
        while (attempts < _maxPlacementAttempts)
        {
            Vector3 potentialPosition = GetPointOnSphereSurface(_planetRadius);

            bool isTooClose = false;
            if (GeneratedPlayerSpawnPoint != null)
                if (Vector3.Distance(potentialPosition, GeneratedPlayerSpawnPoint.position) < _minSpawnPointSpacing)
                    isTooClose = true;
                                
            bool isOverlapping = false;
            if (!isTooClose)                           
                isOverlapping = Physics.CheckSphere(potentialPosition, obstacleCheckRadius, _obstructionLayers);

            if (!isTooClose && !isOverlapping)
            {
                Vector3 normal = (potentialPosition - _planetCenter).normalized; 
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal); 

                GameObject placedObstacle = Instantiate(objectPrefab, potentialPosition, rotation);
                placedObstacle.transform.SetParent(_generatedObjectsHolder); 

                _totalPlacedObstacles++; 
                return true; 
            }

            attempts++;
        }

        Debug.LogWarning($"Failed to place object {objectPrefab.name} after {_maxPlacementAttempts} attempts.", this);
        return false; 
    }

    private void SetLevelParameters(int minHealth, int maxHealth)
    {
        GameManager gameManager = GameManager.gameManagerInstance;
        if (gameManager != null)
        {
            gameManager.player.GetComponent<Health>().SetMaxHealth(Random.Range(minHealth, maxHealth+1));
            gameManager.SetGoalScore(Random.Range(10, 300) * 100);
        }
    }

    private Vector3 GetPointOnSphereSurface(float radius)
    {
        Vector3 randomDirection = Random.onUnitSphere; 
        return _planetCenter + randomDirection * radius; 
    }

    private int GetPlacedObstacleCount()
    {
        return _totalPlacedObstacles;
    }

}