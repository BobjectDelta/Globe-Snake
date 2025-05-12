using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBonus : Bonus
{
    [SerializeField] private float _speedPoints;
    protected override void GrantBonus(GameObject bonusRecipient)
    {
        PlayerMovementController movementController = bonusRecipient.GetComponent<PlayerMovementController>();
        if (movementController != null)
        {
            movementController.ChangeMovementSpeed(_speedPoints);
        }
    }

    protected override void RevertBonus(GameObject bonusRecipient)
    {
        PlayerMovementController movementController = bonusRecipient.GetComponent<PlayerMovementController>();
        if (movementController != null)
        {
            movementController.ChangeMovementSpeed(-_speedPoints);
        }
    }
}
