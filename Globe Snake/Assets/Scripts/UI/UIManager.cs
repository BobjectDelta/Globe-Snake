using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager uiManagerInstance = null;
    [SerializeField] private GameObject _pauseButton;
    [SerializeField] public GameObject _healthCounter;
    [SerializeField] public GameObject _healthBar;
    [SerializeField] private GameObject _scoreCounter;
    [SerializeField] private GameObject _goalScore;
    [SerializeField] private GameObject _compass;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _winMenu;
    [SerializeField] private GameObject _gameOverMenu;

    private void Awake()
    {
        if (uiManagerInstance == null)
            uiManagerInstance = this;
        else
            DestroyImmediate(this);
    }

    public void UpdateHealthCounter(float health)
    {
        if (_healthCounter)
            _healthCounter.GetComponent<TextMeshProUGUI>().text = Mathf.CeilToInt(health).ToString();
        else
            Debug.LogError("Health counter is not assigned in the UIManager.");
    }

    public void UpdateHealthBar(float health)
    {
        if (_healthBar)
            _healthBar.GetComponent<HealthBarManager>().SetCurrentValue(health);
        else
            Debug.LogError("Health bar is not assigned in the UIManager.");
    }

    public void UpdateHealthBarMaxValue(float maxHealth)
    {
        if (_healthBar)
            _healthBar.GetComponent<HealthBarManager>().SetMaxValue(maxHealth);
        else
            Debug.LogError("Health bar is not assigned in the UIManager.");
    }

    public void UpdateScoreCounter(int score)
    {
        _scoreCounter.GetComponent<TextMeshProUGUI>().text = score.ToString();
    }

    public void UpdateGoalScoreText(int goalScore)
    {
        _goalScore.GetComponent<TextMeshProUGUI>().text = goalScore.ToString();
    }

    public void DisplayPauseUI(bool toPause)
    {
        SetActiveGameElements(!toPause);
        _pauseMenu.SetActive(toPause);
    }

    public void DisplayWinUI()
    {
        SetActiveGameElements(false);
        _pauseMenu.SetActive(false);
        _winMenu.SetActive(true);
    }

    public void DisplayGameOverUI()
    {
        SetActiveGameElements(false);
        _pauseMenu.SetActive(false);
        _gameOverMenu.SetActive(true);
    }

    public void SetActiveGameElements(bool active)
    {
        _pauseButton.SetActive(active);
        _healthBar.SetActive(active);
        _healthCounter.SetActive(active);
        _scoreCounter.SetActive(active);
        _goalScore.SetActive(active);
        _compass.SetActive(active);
    }

    public void SetActiveMenuElements(bool active)
    {
        _pauseMenu.SetActive(active);
        _winMenu.SetActive(active);
        _gameOverMenu.SetActive(active);
    }
}
