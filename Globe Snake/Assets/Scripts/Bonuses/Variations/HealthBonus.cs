using UnityEngine;

public class HealthBonus : Bonus
{
    [SerializeField] private float _healPoints;
    protected override void GrantBonus(GameObject healingRecipient)
    {
        Health health = healingRecipient.GetComponent<Health>();
        if (health)
        {
            if (!_isTimed)
                health.Heal(_healPoints);
            else
                health.Heal(_healPoints * Time.deltaTime);

        }
    }

    protected override void RevertBonus(GameObject healingRecipient)
    {
        Health health = healingRecipient.GetComponent<Health>();
        if (health)
            health.TakeDamage(health.GetHealth() - Mathf.RoundToInt(health.GetHealth()));
    }
}
