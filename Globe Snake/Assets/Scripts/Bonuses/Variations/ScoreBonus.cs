using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : Bonus
{
    [SerializeField] private int _scoreBonus = 10;

    protected override void GrantBonus(GameObject bonusRecipient)
    {
        GameManager.gameManagerInstance.AddScore(_scoreBonus);
    }

}
