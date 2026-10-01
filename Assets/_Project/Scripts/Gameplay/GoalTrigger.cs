using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [Tooltip("1 = Player 1 scores, 2 = Player 2 scores")]
    public int scoringPlayer = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        BallController ball = other.GetComponent<BallController>();
        if (ball != null)
            GameManager.Instance?.ScorePoint(scoringPlayer);
    }
}
