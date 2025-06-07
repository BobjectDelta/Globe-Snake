using UnityEngine;
using UnityEngine.EventSystems; 
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LevelEditorController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Transform _planetTransform; 
    [SerializeField] private Transform _placementRayOrigin; 
    [SerializeField] private List<GameObject> _obstaclePrefabs; 
    [SerializeField] private Transform _generatedObjectsParent;
    [SerializeField] private GameObject _spawnPointVisualPrefab;

    [Header("Input System Actions")]
    [SerializeField] private InputActionAsset editorInputActions;
    private InputAction _pointerPositionAction;
    private InputAction _pointerClickAction;

    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSensitivity = 0.3f;
    private Vector3 _lastPointerPosition; 

    [Header("Placement Settings")]
    [SerializeField] private float _minSpawnPointDistance = 10f; 
    [SerializeField] private LayerMask _obstructionLayers; 

    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _scoreSlider;
    
    private Vector3 _planetCenter;
    private float _planetRadius;
    private Transform _playerSpawnPointTransform; 
    private Dictionary<GameObject, string> _placedObstaclePrefabNames = new Dictionary<GameObject, string>();
    private GameObject _selectedObstaclePrefab; 

    void Awake()
    {
        if (_planetTransform == null) 
            Debug.LogError("Planet Transform is not assigned", this);
        if (_placementRayOrigin == null) 
            Debug.LogError("Placement Ray Origin Transform is not assigned", this);

        if (editorInputActions != null)
        {
            var editorMap = editorInputActions.FindActionMap("Editor");
            if (editorMap != null)
            {
                _pointerPositionAction = editorMap.FindAction("PointerPosition");
                _pointerClickAction = editorMap.FindAction("PointerClick");

                if (_pointerPositionAction == null) 
                    Debug.LogError("Input Action 'PointerPosition' not found in map 'Editor'", this);
                if (_pointerClickAction == null) 
                    Debug.LogError("Input Action 'PointerClick' not found in map 'Editor'", this);
            }
            else           
                Debug.LogError("Input Action Map 'Editor' not found in assigned asset", this);       
        }
        else       
            Debug.LogError("Input Action Asset is not assigned in LevelEditorController", this);


        if (_planetTransform != null)
        {
            _planetCenter = _planetTransform.position;
            _planetRadius = _planetTransform.lossyScale.x / 2f;
        }

        if (_generatedObjectsParent == null)
        {
            GameObject parentGO = new GameObject("GeneratedLevelObjects");
            _generatedObjectsParent = parentGO.transform;
        }

        if (_pointerPositionAction != null)
        {
            bool wasEnabled = _pointerPositionAction.enabled;
            if (!wasEnabled) 
                _pointerPositionAction.Enable();
            _lastPointerPosition = _pointerPositionAction.ReadValue<Vector2>(); 
            if (!wasEnabled) 
                _pointerPositionAction.Disable();
        }
    }

    void OnEnable()
    {
        if (editorInputActions != null)
        {
            editorInputActions.Enable();
        }
    }

    void OnDisable()
    {
        if (editorInputActions != null)
        {
            editorInputActions.Disable();
        }
    }

    void Start()
    {
        PlacePlayerSpawnPoint();
    }

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            if (_pointerPositionAction != null && _pointerPositionAction.enabled)           
                _lastPointerPosition = _pointerPositionAction.ReadValue<Vector2>();          
            return; 
        }

        if (_pointerPositionAction != null && _pointerClickAction != null && _pointerPositionAction.enabled && _pointerClickAction.enabled)
        {
            Vector3 currentPointerPosition = _pointerPositionAction.ReadValue<Vector2>(); 

            if (_pointerClickAction.WasPressedThisFrame())            
                _lastPointerPosition = currentPointerPosition;           
            else if (_pointerClickAction.IsPressed())
            {
                Vector3 deltaPointerPosition = currentPointerPosition - _lastPointerPosition;

                float rotationX = deltaPointerPosition.y * _rotationSensitivity;
                float rotationY = deltaPointerPosition.x * _rotationSensitivity;

                _planetTransform.Rotate(Vector3.right, rotationX, Space.World);
                _planetTransform.Rotate(Vector3.up, -rotationY, Space.World);

                _lastPointerPosition = currentPointerPosition;
            }
        }
        
    }

    public void SelectObstacle(GameObject prefab)
    {
        _selectedObstaclePrefab = prefab;
        Debug.Log($"Obstacle selected for placement: {_selectedObstaclePrefab.name}");
    }

    public void PlaceSelectedObstacle()
    {
        if (_selectedObstaclePrefab == null)
        {
            Debug.LogWarning("No obstacle type selected for placement");
            return;
        }

        Vector3 rayOrigin = _placementRayOrigin.position;
        Vector3 rayDirection = _placementRayOrigin.forward;
        Ray ray = new Ray(rayOrigin, rayDirection);
        RaycastHit hit;
       
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) 
        {
            if (hit.collider.transform == _planetTransform) 
            {
                Vector3 potentialPosition = hit.point; 
                bool isSafeToPlace = CheckPlacementObstructions(potentialPosition, _selectedObstaclePrefab);

                if (isSafeToPlace)
                {
                    Vector3 normal = hit.normal;
                    Quaternion placementRotation = Quaternion.FromToRotation(Vector3.up, normal);

                    GameObject newObstacleGO = Instantiate(_selectedObstaclePrefab, potentialPosition, placementRotation);
                    if (_generatedObjectsParent != null)                    
                        newObstacleGO.transform.SetParent(_generatedObjectsParent);
                    
                    if (_selectedObstaclePrefab != null)                   
                        _placedObstaclePrefabNames.Add(newObstacleGO, _selectedObstaclePrefab.name);                    
                    else                  
                        Debug.LogWarning("Placed obstacle but _selectedObstaclePrefab was null? Cannot track prefab name", newObstacleGO);                  

                    Debug.Log($"Placed obstacle: {newObstacleGO.name} at {potentialPosition}");
                }
                else               
                    Debug.LogWarning($"Placement position {potentialPosition} is obstructed or too close to spawn point", this);                
            }
            else
                Debug.Log("Raycast hit something, but not the planet" + hit.collider);
        }
    }

    private void PlacePlayerSpawnPoint()
    {
        if (_planetTransform == null)
        {
            Debug.LogError("Cannot place spawn point, planet transform is null", this);
            return;
        }
        if (_spawnPointVisualPrefab == null)
        {
            Debug.LogError("Spawn Point Visual Prefab is not assigned! Cannot place spawn point visual", this);
            return;
        }

        Vector3 spawnPosition = GetPointOnSphereSurface(_planetRadius);
        Vector3 normal = (spawnPosition - _planetCenter).normalized;
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, normal);

        GameObject spawnPointGO = Instantiate(_spawnPointVisualPrefab, spawnPosition, spawnRotation);
        spawnPointGO.name = "PlayerSpawnPoint_Editor"; 

        if (_generatedObjectsParent != null)        
            spawnPointGO.transform.SetParent(_generatedObjectsParent);
        
        _playerSpawnPointTransform = spawnPointGO.transform;

        Debug.Log($"Editor: Placed Player Spawn Point at {spawnPosition}");
    }

    private bool CheckPlacementObstructions(Vector3 potentialPosition, GameObject prefabToPlace)
    {
        if (_playerSpawnPointTransform != null)
            if (Vector3.Distance(potentialPosition, _playerSpawnPointTransform.position) < _minSpawnPointDistance)
            {
                Debug.LogWarning($"Placement position {potentialPosition} is too close to the Player Spawn Point");
                return false;
            }

        Collider prefabCollider = prefabToPlace.GetComponent<Collider>();
        float objectCheckRadius = 1f; 
        if (prefabCollider != null)       
            objectCheckRadius = prefabToPlace.transform.lossyScale.magnitude;      
        else       
            Debug.LogWarning($"Prefab '{prefabToPlace.name}' is missing a Collider. Using default check radius ({objectCheckRadius})", prefabToPlace);               

        bool isOverlappingObstructions = Physics.CheckSphere(potentialPosition, objectCheckRadius, _obstructionLayers);
        Color debugColor = isOverlappingObstructions ? Color.red : Color.green;
        Debug.DrawLine(potentialPosition, _placementRayOrigin.position * objectCheckRadius * 2, debugColor, 1f); 

        if (isOverlappingObstructions)
        {
            Debug.LogWarning($"Placement position {potentialPosition} overlaps with existing obstacles or bonuses", this);
            return false;
        }

        return true;
    }

    private Vector3 GetPointOnSphereSurface(float radius)
    {
        if (_planetTransform == null) 
            return Vector3.zero; 
        Vector3 randomDirection = Random.onUnitSphere; 

        return _planetCenter + randomDirection * radius; 
    }

    public void SaveLevelToSlot(int slotNumber, string levelName)
    {
        if (slotNumber <= 0)
        {
            Debug.LogError($"Invalid slot number provided: {slotNumber}");
            return;
        }

        LevelData dataToSave = new LevelData(levelName); 

        if (_playerSpawnPointTransform != null)        
            dataToSave.spawnPoint = new SpawnPointData(_playerSpawnPointTransform);      
        else      
            Debug.LogWarning($"Player Spawn Point Transform is null. Saving level data for Slot {slotNumber} without spawn point data");
        
        foreach (KeyValuePair<GameObject, string> entry in _placedObstaclePrefabNames)
        {
            GameObject placedObstacleGO = entry.Key;
            string prefabName = entry.Value; 

            if (placedObstacleGO != null)
            {
                ObstacleData obstacleData = new ObstacleData(placedObstacleGO, prefabName);
                dataToSave.obstacles.Add(obstacleData);
            }            
        }        

        dataToSave.maxPlayerHealth = _healthSlider.value; 
        dataToSave.winScore = Mathf.CeilToInt(_scoreSlider.value * 1000);         
        
        string jsonString = JsonUtility.ToJson(dataToSave, true);
       
        string directoryPath = Path.Combine(Application.persistentDataPath, "Levels"); 
        string fileName = $"slot_{slotNumber}.json"; 
        string filePath = Path.Combine(directoryPath, fileName);

        if (!Directory.Exists(directoryPath))        
            Directory.CreateDirectory(directoryPath);

        try
        {
            File.WriteAllText(filePath, jsonString);
            Debug.Log($"Level '{levelName}' saved successfully to Slot {slotNumber} ({filePath})");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save level to Slot {slotNumber} ({filePath}): {e.Message}");
        }
    }

}