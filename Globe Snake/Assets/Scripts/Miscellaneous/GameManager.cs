using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManagerInstance = null;

    [SerializeField] public GameObject player = null;
    [SerializeField] private Vector3 _playerSpawnPoint;
    [SerializeField] private int _scoreGoal = 1000;
    [SerializeField] private float _scorePerTick = 10f;
    [SerializeField] private float _scoreTickInterval = 1000f;

    private int _currentScore = 0;
    private float _scoreTimer = 0f;
    private float _bonusScoreMultiplier = 1f;
    private PlayerMovementController _playerMovementController = null;
    private UIManager _uiManager;

    private bool _isPaused = false;

    private void Awake()
    {
        if (gameManagerInstance == null)       
            gameManagerInstance = this;      
        else if (gameManagerInstance != this)        
            Destroy(gameObject);      
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _playerMovementController = player.GetComponent<PlayerMovementController>();
        player.transform.position = _playerSpawnPoint;

        Time.timeScale = 1;

        _uiManager = UIManager.uiManagerInstance;
        _scoreTimer = Time.time + _scoreTickInterval;

        _uiManager.SetActiveMenuElements(false);
        _uiManager.SetActiveGameElements(true);
        _uiManager.UpdateScoreCounter(_currentScore);
        _uiManager.UpdateGoalScoreText(_scoreGoal);
        _uiManager.UpdateHealthCounter(player.GetComponent<Health>().GetHealth());
        _uiManager.UpdateHealthBarMaxValue(player.GetComponent<Health>().GetMaxHealth());
        _uiManager.UpdateHealthBar(player.GetComponent<Health>().GetHealth());
    }

    private void Update()
    {
        if (Time.timeScale > 0 && Time.time >= _scoreTimer)
        {
            float speedMultiplier = _playerMovementController.GetMovementSpeed() / 100;
            float totalMultiplier = speedMultiplier * _bonusScoreMultiplier;

            if (totalMultiplier < 0.1f)
                totalMultiplier = 0.1f;
            AddScore(Mathf.CeilToInt(_scorePerTick * totalMultiplier));

            _scoreTimer = Time.time + _scoreTickInterval;
        }
    }

    public void AddScore(int amount)
    {
        _currentScore += amount;
        if (_uiManager != null)
            _uiManager.UpdateScoreCounter(_currentScore);

        CheckWinCondition();
    }

    public void ApplyScoreMultiplier(float multiplier)
    {
        Debug.Log($"Score multiplier applied: {multiplier}");
         _bonusScoreMultiplier *= multiplier;
    }

    public void TriggerGameOver()
    {
        Time.timeScale = 0;
        _uiManager.DisplayGameOverUI();
    }

    public void SetGoalScore(int goalScore)
    {
        _scoreGoal = goalScore;
        Debug.Log($"Game Goal Score set to: {_scoreGoal}");
        if (_uiManager != null)        
            _uiManager.UpdateGoalScoreText(goalScore);        
    }

    public Vector3 GetPlayerSpawnPoint()
    {
        if (_playerSpawnPoint == null)
        {
            Debug.LogError("Player spawn point is not set in GameManager");
            return Vector3.zero;
        }
        return _playerSpawnPoint;
    }

    public void SetPlayerSpawnPoint(Vector3 spawnPoint)
    {
        _playerSpawnPoint = spawnPoint;
        Debug.Log("Player spawn point set in GameManager");
    }

    public void CheckWinCondition()
    {
        if (_currentScore >= _scoreGoal)
            TriggerWin();
    }

    private void TriggerWin()
    {
        Time.timeScale = 0;
        _uiManager.DisplayWinUI();
    }

    public void TogglePause()
    {
        if (!_isPaused)
            Pause();
        else
            Resume();
    }

    private void Pause()
    {
        Time.timeScale = 0;
        _isPaused = true;
        _uiManager.DisplayPauseUI(_isPaused);
    }

    private void Resume()
    {
        Time.timeScale = 1;
        _isPaused = false;
        _uiManager.DisplayPauseUI(_isPaused);
    }
}