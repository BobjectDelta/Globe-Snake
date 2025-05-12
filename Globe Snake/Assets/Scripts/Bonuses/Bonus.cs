using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    private GameObject _bonusRecipient;

    [SerializeField] protected float _effectTime;
    protected float _remainingEffectTime = 0;

    [SerializeField] protected bool _isTimed = false;
    [SerializeField] protected bool _isOverTime = false;

    private bool _isBuffed;


    private void Start()
    {
        _isBuffed = false;
    }

    private void Update()
    {
        if (_remainingEffectTime > 0)
        {
            if (!_isBuffed || _isOverTime)
                GrantBonus(_bonusRecipient);
            _isBuffed = true;
            _remainingEffectTime -= Time.deltaTime;
        }
        else if (_isBuffed && _remainingEffectTime < 0)
        {
            RevertBonus(_bonusRecipient);
            _isBuffed = false;
        }
    }

    private void OnTriggerEnter(Collider collisionObject)
    {
        if (collisionObject.GetComponent<PlayerMovementController>())
        {
            _bonusRecipient = collisionObject.gameObject;
            if (!_isTimed)
                GrantBonus(_bonusRecipient);
            else
                _remainingEffectTime = _effectTime;

            Debug.Log("Remaining effect time: " + _remainingEffectTime);
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject, _effectTime + 0.01f);
        }
    }

    protected virtual void GrantBonus(GameObject bonusRecipient) { }
    protected virtual void RevertBonus(GameObject bonusRecipient) { }

    public virtual void SetEffectTime(float time)
    {
        _effectTime = time;
    }
}
