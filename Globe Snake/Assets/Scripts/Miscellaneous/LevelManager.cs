using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Transform _obstacleParentPlayScene; 
    private Vector3 _playerSpawnPoint; 

    [Header("Prefab Mapping")]
    [SerializeField] private List<PrefabNameMapping> _obstaclePrefabMapList;
    private Dictionary<string, GameObject> _obstaclePrefabMap; 

    [Header("Level Loading Settings")]
    [SerializeField] private int _defaultSlotToLoad = 1;

    private const string PlayerPrefsSlotKey = "SelectedLevelSlot";

    private GameManager _gameManager;
    private GameObject _player; 
    private float _maxPlayerHealth = 5f; 
    private int _winScore = 1000; 


    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)       
            Debug.LogError("Player GameObject with 'Player' tag not found in the scene", this);

        _obstaclePrefabMap = new Dictionary<string, GameObject>();
        if (_obstaclePrefabMapList != null)
        {
            foreach (var entry in _obstaclePrefabMapList)
            {
                if (entry.prefab != null && !string.IsNullOrEmpty(entry.name))
                {
                    if (!_obstaclePrefabMap.ContainsKey(entry.name))                   
                        _obstaclePrefabMap.Add(entry.name, entry.prefab);                  
                    else                    
                        Debug.LogWarning($"Duplicate prefab name mapping found for '{entry.name}' in LevelManager");                  
                }
                else               
                    Debug.LogWarning("Prefab name mapping entry is incomplete (name or prefab missing) in LevelManager");               
            }
        }
        else       
            Debug.LogWarning("Obstacle Prefab Map List is not assigned in LevelManager, cannot load obstacles");

        int slotToLoad = PlayerPrefs.GetInt(PlayerPrefsSlotKey, _defaultSlotToLoad);
        Debug.Log($"LevelManager: Retrieved Slot {slotToLoad} from PlayerPrefs");

        if (_obstacleParentPlayScene == null)
        {
            GameObject parentGO = GameObject.Find("GeneratedLevelObjects");
            if (parentGO != null) 
                _obstacleParentPlayScene = parentGO.transform;
            else 
                Debug.LogWarning("Obstacle Parent in Play Scene ('GeneratedLevelObjects') not assigned and not found by name! Instantiated obstacles won't be parented", this);
        }

        if (slotToLoad != -1) 
        {
            Debug.Log($"LevelManager: Loading level from Slot {slotToLoad}...");
            LoadLevelFromSlot(slotToLoad);
        }
        else
        {
            Debug.LogWarning($"LevelManager: No specific slot selected by GameManager. Loading default slot {_defaultSlotToLoad}");
            LoadLevelFromSlot(_defaultSlotToLoad); 
        }
    }

    private void Start()
    {
        _gameManager = GameManager.gameManagerInstance;
        _gameManager.SetPlayerSpawnPoint(_playerSpawnPoint); 
        _gameManager.SetGoalScore(_winScore); 
        _player.GetComponent<Health>().SetMaxHealth(_maxPlayerHealth); 
        _player.GetComponent<Health>().Heal(_maxPlayerHealth); 
    }

    private void LoadLevelFromSlot(int slotNumber)
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "Levels");
        string fileName = $"slot_{slotNumber}.json";
        string filePath = Path.Combine(directoryPath, fileName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Level save file not found for Slot {slotNumber}: {filePath}. Cannot load level");
            return;
        }

        string jsonString = "";
        try
        {
            jsonString = File.ReadAllText(filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to read level file for Slot {slotNumber}: {e.Message}", this);
            return;
        }

        LevelData loadedData = null;
        try
        {
            loadedData = JsonUtility.FromJson<LevelData>(jsonString);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to deserialize level data for Slot {slotNumber}: {e.Message}", this);
            return;
        }

        if (loadedData != null)
        {
            Debug.Log($"Successfully loaded data for level '{loadedData.levelName}' from Slot {slotNumber}");
            InstantiateLevelObjects(loadedData);
            ApplyLevelParameters(loadedData);
        }
    }

    private void InstantiateLevelObjects(LevelData data)
    {
        if (data.spawnPoint != null)
        {
            Vector3 spawnPosition = data.spawnPoint.position.ToVector3();
            Quaternion spawnRotation = data.spawnPoint.rotation.ToQuaternion();

            _playerSpawnPoint = spawnPosition; 
        }
        else       
            Debug.LogWarning("Spawn point data missing or Player object not found");
        

        if (data.obstacles != null && _obstaclePrefabMap != null && _obstacleParentPlayScene != null)
        {
            Debug.Log($"Instantiating {data.obstacles.Count} obstacles from loaded data...");
            foreach (ObstacleData obstacleData in data.obstacles)
            {
                if (_obstaclePrefabMap.TryGetValue(obstacleData.prefabName, out GameObject obstaclePrefab))
                {
                    Vector3 obstaclePosition = obstacleData.position.ToVector3();
                    Quaternion obstacleRotation = obstacleData.rotation.ToQuaternion();

                    GameObject instantiatedObstacle = Instantiate(obstaclePrefab, obstaclePosition, obstacleRotation);
                    instantiatedObstacle.transform.SetParent(_obstacleParentPlayScene);         
                }
                else                
                    Debug.LogWarning($"Prefab '{obstacleData.prefabName}' not found in prefab map, cannot instantiate obstacle");
            }
        }
        else
        {
            if (data.obstacles == null) 
                Debug.LogWarning("Level data has no obstacle data");
            if (_obstaclePrefabMap == null) 
                Debug.LogError("Obstacle Prefab Map is null, cannot instantiate obstacles");
            if (_obstacleParentPlayScene == null) 
                Debug.LogError("Obstacle Parent in Play Scene is null, cannot instantiate obstacles");
        }
    }

    private void ApplyLevelParameters(LevelData data)
    {
        if (_player != null)
        {
            Health playerHealth = _player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.SetMaxHealth(data.maxPlayerHealth); 
                playerHealth.Heal(data.maxPlayerHealth); 
                Debug.Log($"Applied loaded Player Max Health: {data.maxPlayerHealth}");
            }
            else 
                Debug.LogWarning("Player object missing Health component, cannot apply health parameters", _player);
        }
        else 
            Debug.LogWarning("Player object is null, cannot apply max health parameters");


        if (_gameManager != null)
        {
            _gameManager.SetGoalScore(data.winScore); 
            Debug.Log($"Applied loaded Win Score to GameManager: {data.winScore}");            
        }
        else 
            Debug.LogError("GameManager instance is null, cannot apply win score parameter");

        _maxPlayerHealth = data.maxPlayerHealth; 
        _winScore = data.winScore; 
    }

}