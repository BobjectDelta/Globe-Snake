using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class SaveSlotButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int _slotNumber; 
    [SerializeField] private TMP_InputField _levelNameInput; 
    [SerializeField] private TextMeshProUGUI _slotNameDisplay;

    private LevelEditorController _editorController; 

    void Awake()
    {
        Button btn = GetComponent<Button>();
        if (btn == null)
            Debug.LogError("SaveSlotButton script requires a Button component", this);

        _editorController = FindObjectOfType<LevelEditorController>();
        if (_editorController == null) 
            Debug.LogError("LevelEditorController not found in the scene", this);

        if (_slotNameDisplay != null)       
            _slotNameDisplay.text = $"{_slotNumber}";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            string levelName = GetSlotNumber().ToString(); 

            if (_editorController != null)
            {
                Debug.Log($"Attempting to save level '{levelName}' to Slot {_slotNumber}");
                _editorController.SaveLevelToSlot(_slotNumber, levelName);

                if (_slotNameDisplay != null)                
                    _slotNameDisplay.text = levelName;               
            }
            else           
                Debug.LogError("Editor Controller is null, cannot save");
            
        }
    }

    public int GetSlotNumber()
    {
        return _slotNumber;
    }
}