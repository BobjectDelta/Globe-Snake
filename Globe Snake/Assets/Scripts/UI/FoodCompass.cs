using UnityEngine;
using System.Collections.Generic; 
using TMPro; 
using UnityEngine.UI; 

public class FoodCompass : MonoBehaviour
{
    [SerializeField] private Transform _player; 
    [SerializeField] private Transform _bonusHolder; 
    [SerializeField] private RectTransform _compassArrow;
    [SerializeField] private RectTransform _idleCompass;

    [SerializeField] private float _updateInterval = 0.2f;
    private float _updateTimer;

    private Food _nearestFood = null; 


    void Start()
    {
        if (_player == null) 
            Debug.LogError("Player Head Transform not assigned to FoodCompass!", this);
        if (_bonusHolder == null) 
            Debug.LogError("Bonus Holder Transform not assigned to FoodCompass!", this);
        if (_compassArrow == null) 
            Debug.LogError("Compass Arrow RectTransform not assigned to FoodCompass!", this);
        if (_idleCompass == null)
            Debug.LogError("Idle Compass RectTransform not assigned to FoodCompass!", this);

        if (_compassArrow != null)
            _compassArrow.gameObject.SetActive(false); 
        if (_idleCompass != null)
            _idleCompass.gameObject.SetActive(true); 

        _updateTimer = 0f; 
        UpdateNearestFood(); 
    }

    void Update()
    {
        if (Time.time >= _updateTimer)
        {
            UpdateNearestFood();
            _updateTimer = Time.time + _updateInterval;
        }

        if (_nearestFood != null)
        {
            if (!_nearestFood.gameObject.GetComponent<Collider>().enabled)
            {
                _updateTimer = Time.time + _updateInterval;

                _compassArrow.gameObject.SetActive(false);
                _idleCompass.gameObject.SetActive(true);
                return;
            }

            Vector3 foodDirection = _nearestFood.transform.position - _player.position;
            Vector3 localFoodDirection = _player.InverseTransformDirection(foodDirection);
            Vector3 flatLocalDirection = new Vector3(localFoodDirection.x, 0, localFoodDirection.z);

            if (flatLocalDirection.sqrMagnitude < 0.0001f)            
                _compassArrow.localEulerAngles = new Vector3(0, 0, 0);                          
            else
            {
                float angle = Vector3.SignedAngle(Vector3.forward, flatLocalDirection, Vector3.up);
                _compassArrow.localEulerAngles = new Vector3(0, 0, -angle);

                _idleCompass.gameObject.SetActive(false);
                _compassArrow.gameObject.SetActive(true);
            }
        }
        else
        {
            _compassArrow.gameObject.SetActive(false);
            _idleCompass.gameObject.SetActive(true);
        }
    }

    private void UpdateNearestFood()
    {
        _nearestFood = null; 
        float minDistance = Mathf.Infinity; 

        if (_bonusHolder == null || _player == null) 
            return;

        for (int i = 0; i < _bonusHolder.childCount; i++)
        {
            Transform bonusChild = _bonusHolder.GetChild(i);
            if (bonusChild.gameObject.activeInHierarchy)
            {
                float curMinDistance = (bonusChild.position - _player.position).sqrMagnitude;
                if (curMinDistance < minDistance)
                {
                    minDistance = curMinDistance; 
                    _nearestFood = bonusChild.GetComponent<Food>(); ; 
                }
            }
        }       
    }

}
