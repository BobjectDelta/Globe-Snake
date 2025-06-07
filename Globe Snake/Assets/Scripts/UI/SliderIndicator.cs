using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))] 
public class SliderIndicator : MonoBehaviour
{
    [SerializeField] private Slider _slider; 
    [SerializeField] private bool _isPercent = false; 
    private TextMeshProUGUI _textComponent; 

    void Start()
    {
        _textComponent = GetComponent<TextMeshProUGUI>();
        if (_slider != null)
        {
            _slider.onValueChanged.AddListener(delegate { UpdateTextValue(); });
            UpdateTextValue(); 
        }
    }

    public void UpdateTextValue()
    {
        if (_isPercent)
        {
            float percentageValue = (_slider.value - _slider.minValue) / (_slider.maxValue - _slider.minValue) * 100; 
            _textComponent.text = Mathf.Abs(percentageValue).ToString("F0") + "%"; 
        }
        else        
            _textComponent.text = _slider.value.ToString("F0"); 
    }
}
