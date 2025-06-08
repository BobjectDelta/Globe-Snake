using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

[RequireComponent(typeof(Button))]
public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private int _slotNumber; 
    [SerializeField] private TextMeshProUGUI _buttonText; 

    [Header("Scene Settings")]
    [SerializeField] private string _gameplaySceneName = "LoadTestScene"; 

    private const string PlayerPrefsSlotKey = "SelectedLevelSlot";
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        if (_button == null)
            Debug.LogError("LevelSelectButton requires a Button component", this);
        else       
            UpdateButtonDisplay();       
    }

    public void SelectLevelBySlot()
    {
        PlayerPrefs.SetInt(PlayerPrefsSlotKey, _slotNumber);
        PlayerPrefs.Save(); 
        Debug.Log($"Selected Slot {_slotNumber} and saved to PlayerPrefs");
        SceneManager.LoadScene(_gameplaySceneName); 
    }

    public void SelectNextLevel()
    {
        _slotNumber = PlayerPrefs.GetInt(PlayerPrefsSlotKey, 1);
        int nextSlot = (_slotNumber % 4) + 1;
        while (!IsLevelAvailable(nextSlot))
        {
            nextSlot = (nextSlot % 4) + 1; 
            if (nextSlot == _slotNumber)           
                Debug.LogWarning("No available levels found in all slots");          
        }
        PlayerPrefs.SetInt(PlayerPrefsSlotKey, nextSlot);
        PlayerPrefs.Save();
        Debug.Log($"Selected Next Slot {nextSlot} and saved to PlayerPrefs");
        SceneManager.LoadScene(_gameplaySceneName);
    }

    public void UpdateButtonDisplay()
    {
        if (_buttonText != null)
        {
            string directoryPath = Path.Combine(Application.persistentDataPath, "Levels");
            string fileName = $"slot_{_slotNumber}.json";
            string filePath = Path.Combine(directoryPath, fileName);

            Debug.Log($"Checking for level: {_slotNumber} " + File.Exists(filePath));
            if (File.Exists(filePath))
            {
                try
                {
                    string jsonString = File.ReadAllText(filePath);
                    LevelData tempLevelData = JsonUtility.FromJson<LevelData>(jsonString);
                    if (tempLevelData != null && !string.IsNullOrEmpty(tempLevelData.levelName))                   
                        _buttonText.text = tempLevelData.levelName;                    
                    else                    
                        _buttonText.text = $"Slot {_slotNumber} (Unnamed)";                     
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error reading level name for Slot {_slotNumber}: {e.Message}");
                    _buttonText.text = $"Slot {_slotNumber} (Read Error)"; 
                }
                _button.interactable = true; 
            }
            else
            {
                _buttonText.text = $"{_slotNumber}";
                _button.interactable = false; 
            }
        }
        else       
            Debug.LogWarning("Button TextMeshProUGUI is not assigned on LevelSelectButton", this);
    }

    private bool IsLevelAvailable(int slotNumber)
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "Levels");
        string fileName = $"slot_{slotNumber}.json";
        string filePath = Path.Combine(directoryPath, fileName);
        return File.Exists(filePath);
    }
}