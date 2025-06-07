using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _player;
    [SerializeField] private Health _health;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _health = _player.GetComponent<Health>();
        SetMaxValue(_health.GetMaxHealth());
        SetCurrentValue(_health.GetHealth());
    }


    public void SetMaxValue(float healthPoints)
    {
        _slider.maxValue = healthPoints;
    }

    public void SetCurrentValue(float healthPoints)
    {
        _slider.value = healthPoints;
    }
}

