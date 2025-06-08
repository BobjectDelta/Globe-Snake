using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float _health;
    [SerializeField] private float _maxHealth;
    [SerializeField] private bool _isInvincible = false;
    [SerializeField] private float _invincibilityTime = 1f;

    private float _invincibilityTimeCounter = 0f;
    private void Start()
    {
        if (_health > _maxHealth)
            _health = _maxHealth;

        _isInvincible = true;
        _invincibilityTimeCounter = Time.time + _invincibilityTime;
    }

    private void Update()
    {
        if (_isInvincible)       
            if (Time.time > _invincibilityTimeCounter)
            {
                _isInvincible = false;
                Debug.Log("Invincibility ended");
            }        
    }

    public void TakeDamage(float damage)
    {
        if (!_isInvincible)
        {
            _health -= damage;
            Debug.Log("Damage taken: " + damage);
            if (gameObject.CompareTag("Player"))
                UIManager.uiManagerInstance._healthBar.GetComponent<HealthBarManager>().SetCurrentValue(_health);

            if (_health <= 0)
            {
                _health = 0;
                Die();
                return;
            }

            _isInvincible = true;
            _invincibilityTimeCounter = Time.time + _invincibilityTime;
        }
        else
        {
            if (Time.time > _invincibilityTimeCounter)
            {
                _isInvincible = false;
                Debug.Log("Invincibility ended");
            }
        }
    }

    private void Die()
    {
        Debug.Log("Die Triggered");
        if (gameObject.CompareTag("Player") && GameManager.gameManagerInstance != null)
        {
            Debug.Log("Game Over Triggered");
            GameManager.gameManagerInstance.TriggerGameOver();
            return;
        }
        Destroy(gameObject);
    }

    public void Heal(float healAmount)
    {
        _health += healAmount;
        if (_health > _maxHealth)
            _health = _maxHealth;
        if (gameObject.CompareTag("Player") && GameManager.gameManagerInstance != null)
            UIManager.uiManagerInstance._healthBar.GetComponent<HealthBarManager>().SetCurrentValue(_health);
        //if (gameObject.CompareTag("Player")) 
        //    UIManager.uiManagerInstance.UpdateHealthCounter(_health);

        Debug.Log("Healed to: " + _health);
    }

    public float GetHealth()
    {
        return _health;
    }

    public float GetMaxHealth()
    {
        return _maxHealth;
    }

    public void SetMaxHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
    }

    public bool GetIsInvincible()
    {
        return _isInvincible;
    }

}
