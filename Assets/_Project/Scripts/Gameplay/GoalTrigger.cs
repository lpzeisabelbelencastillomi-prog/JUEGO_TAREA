using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [Tooltip("1 = Player 1 scores, 2 = Player 2 scores")]
    [Range(1, 2)] public int scoringPlayer = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out BallController ball) && ball.IsActive)
            GameManager.Instance?.ScorePoint(scoringPlayer);
    }
}
