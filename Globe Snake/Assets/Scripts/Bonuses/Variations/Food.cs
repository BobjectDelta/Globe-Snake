using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Bonus
{
    protected override void GrantBonus(GameObject bonusRecipient)
    {
        SnakeBodyController snakeBodyController = bonusRecipient.GetComponent<SnakeBodyController>();
        if (snakeBodyController != null)
        {
            snakeBodyController.AddSegment();
        }
    }
}
