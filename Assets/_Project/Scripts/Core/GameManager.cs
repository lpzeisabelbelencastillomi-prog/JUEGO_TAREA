using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int maxScore = 5;
    public BallController ball;
    public ScoreManager scoreManager;
    public GameObject pausePanel;
    public GameObject winPanel;
    public Text winnerText;
    private bool gameEnded;
    private bool waitingLaunch;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        scoreManager.ResetScore();
        WireButtons();
        ball.ResetBall();
        StartCoroutine(LaunchAfterDelay(0.8f));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameEnded)
            TogglePause();
    }

    void WireButtons()
    {
        Bind("ResumeButton", Resume);
        Bind("RestartButton", RestartMatch);
        Bind("PauseMenuButton", BackToMenu);
        Bind("RematchButton", RestartMatch);
        Bind("WinMenuButton", BackToMenu);
    }

    void Bind(string name, UnityEngine.Events.UnityAction action)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) return;
        Button b = go.GetComponent<Button>();
        if (b != null) b.onClick.AddListener(action);
    }

    IEnumerator LaunchAfterDelay(float seconds)
    {
        if (waitingLaunch) yield break;
        waitingLaunch = true;
        yield return new WaitForSeconds(seconds);
        waitingLaunch = false;
        if (!gameEnded) ball.Launch();
    }

    public void ScorePoint(int player)
    {
        if (gameEnded) return;
        ball.ResetBall();
        int score = scoreManager.AddPoint(player);
        AudioManager.Instance?.PlayGoal();
        if (score >= maxScore)
        {
            gameEnded = true;
            if (winnerText != null) winnerText.text = "PLAYER " + player + " GANA";
            if (winPanel != null) winPanel.SetActive(true);
            AudioManager.Instance?.PlayWin();
        }
        else
        {
            StartCoroutine(LaunchAfterDelay(0.7f));
        }
    }

    public void TogglePause()
    {
        bool pause = Time.timeScale > 0f;
        Time.timeScale = pause ? 0f : 1f;
        if (pausePanel != null) pausePanel.SetActive(pause);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        AudioManager.Instance?.PlayClick();
    }

    public void RestartMatch()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
