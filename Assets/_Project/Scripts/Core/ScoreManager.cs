using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text player1Text;
    public Text player2Text;

    public int Player1Score { get; private set; }
    public int Player2Score { get; private set; }

    public void ResetScore()
    {
        Player1Score = 0;
        Player2Score = 0;
        Refresh();
    }

    public int AddPoint(int player)
    {
        if (player == 1) Player1Score++;
        else if (player == 2) Player2Score++;
        Refresh();
        return GetScore(player);
    }

    public int GetScore(int player) => player == 1 ? Player1Score : Player2Score;

    public void Refresh()
    {
        if (player1Text != null) player1Text.text = Player1Score.ToString();
        if (player2Text != null) player2Text.text = Player2Score.ToString();
    }
}
