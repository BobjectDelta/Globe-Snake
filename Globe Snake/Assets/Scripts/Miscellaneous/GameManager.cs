using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManagerInstance = null;

    [SerializeField] public GameObject player = null;
    [SerializeField] private Transform _playerSpawnPoint;
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
        else
            DestroyImmediate(this);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _playerMovementController = player.GetComponent<PlayerMovementController>();

        Time.timeScale = 1;

        _uiManager = UIManager.uiManagerInstance;
        _scoreTimer = Time.time + _scoreTickInterval;

        _uiManager.SetActiveMenuElements(false);
        _uiManager.SetActiveGameElements(true);
        _uiManager.UpdateScoreCounter(_currentScore);
        _uiManager.UpdateHealthCounter(player.GetComponent<Health>().GetHealth());
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
        //Debug.Log($"Added {amount} score. Total Score: {_currentScore}");

        if (_uiManager)       
            _uiManager.UpdateScoreCounter(_currentScore); 
                                                          
        CheckWinCondition();
    }

    public void ApplyScoreMultiplier(float multiplier)
    {
        _bonusScoreMultiplier *= multiplier;
        Debug.Log($"Bonus score multiplier applied: {multiplier}");
    }

    public Transform GetPlayerSpawnPoint()
    {
        return _playerSpawnPoint;
    }

    public void CheckWinCondition()
    {
        if (_currentScore >= _scoreGoal)
            TriggerWin();
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

    private void TriggerWin()
    {
        Time.timeScale = 0;
        _uiManager.DisplayWinUI();
    }

    public void TriggerGameOver()
    {
        Time.timeScale = 0;
        _uiManager.DisplayGameOverUI();
    }
}
