using UnityEngine;

public class ScoreBonus : Bonus
{
    [SerializeField] private int _scoreBonus = 100;

    protected override void GrantBonus(GameObject bonusRecipient)
    {
        GameManager.gameManagerInstance.AddScore(_scoreBonus);
    }

}
