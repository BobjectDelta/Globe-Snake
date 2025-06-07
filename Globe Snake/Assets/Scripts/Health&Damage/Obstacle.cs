using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float _damageAmount = 1f;
    [SerializeField] private bool _shouldTeleportPlayer = false;

    private void OnTriggerEnter(Collider playerCollider)
    {
        TryDamagePlayer(playerCollider);
    }

    private void OnTriggerStay(Collider playerCollider)
    {
        TryDamagePlayer(playerCollider);
    }

    private void TryDamagePlayer(Collider playerCollider)
    {
        SnakeBodyController snakeController = playerCollider.GetComponent<SnakeBodyController>(); 

        if (snakeController != null)
        {
            Health playerHealth = playerCollider.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_damageAmount);
                if (_shouldTeleportPlayer && playerHealth.GetHealth() > 0)               
                    if (GameManager.gameManagerInstance != null)                   
                        playerCollider.transform.position = GameManager.gameManagerInstance.GetPlayerSpawnPoint();
                    else
                        Debug.LogWarning("GameManager instance is null, can't teleport player");
            }
        }
    }
}
