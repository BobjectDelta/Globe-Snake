using UnityEngine;

public class ScoreMultiplierBonus : Bonus
{
    [SerializeField] private float _scoreMultiplier = 10f;
    protected override void GrantBonus(GameObject bonusRecipient)
    {
        GameManager.gameManagerInstance.ApplyScoreMultiplier(_scoreMultiplier);
    }

    protected override void RevertBonus(GameObject bonusRecipient)
    {
        GameManager.gameManagerInstance.ApplyScoreMultiplier(1/_scoreMultiplier);
    }
}
