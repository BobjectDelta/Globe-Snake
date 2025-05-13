using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float _damageAmount = 1f;
    [SerializeField] private bool _shouldTeleportPlayer = false;

    private void OnTriggerEnter(Collider other)
    {
        TryDamagePlayer(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider potentialRecipient)
    {
        SnakeBodyController playerHeadController = potentialRecipient.GetComponent<SnakeBodyController>();

        if (playerHeadController != null)
        {
            Health playerHealth = potentialRecipient.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_damageAmount);
                // Debug.Log($"{gameObject.name} (Obstacle) attempting to damage {potentialRecipient.gameObject.name}.");
                if (_shouldTeleportPlayer && playerHealth.GetHealth() > 0)               
                    if (GameManager.gameManagerInstance != null)                   
                        potentialRecipient.transform.position = GameManager.gameManagerInstance.GetPlayerSpawnPoint().position;
                    else
                        Debug.LogWarning("GameManager instance is null. Cannot teleport player.");
            }
        }
    }
}
