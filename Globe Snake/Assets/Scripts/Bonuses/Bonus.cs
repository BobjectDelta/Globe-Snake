using UnityEngine;

public class Bonus : MonoBehaviour
{
    private GameObject _bonusRecipient;

    [SerializeField] protected float _effectTime;
    protected float _remainingEffectTime = 0;

    [SerializeField] protected bool _isTimed = false;
    [SerializeField] protected bool _isOverTime = false;
    [SerializeField] protected GameObject _soundEffect;
    [SerializeField] protected GameObject _visualEffect;

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

            if (_visualEffect != null)
                Instantiate(_visualEffect, transform.position, Quaternion.identity);
            if (_soundEffect != null)
                Instantiate(_soundEffect, transform.position, Quaternion.identity);

            Debug.Log("Remaining effect time: " + _remainingEffectTime);

            DisableVisualsAndCollider();
            Destroy(gameObject, _effectTime + 0.01f);
        }
    }

    private void DisableVisualsAndCollider()
    {
        Collider mainCollider = GetComponent<Collider>();
        if (mainCollider != null)       
            mainCollider.enabled = false;
        
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)       
            renderer.enabled = false;     

        Light[] lights = GetComponentsInChildren<Light>();
        foreach (Light light in lights)       
            light.enabled = false;       
    }

    protected virtual void GrantBonus(GameObject bonusRecipient) { }
    protected virtual void RevertBonus(GameObject bonusRecipient) { }

    public virtual void SetEffectTime(float time)
    {
        _effectTime = time;
    }
}
