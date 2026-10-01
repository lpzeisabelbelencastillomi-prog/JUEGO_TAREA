using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Match")]
    [Min(1)] public int maxScore = 5;
    public BallController ball;
    public ScoreManager scoreManager;

    [Header("HUD")]
    public Text countdownText;
    public Text roundStatusText;

    [Header("Pause")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button restartButton;
    public Button pauseMenuButton;

    [Header("Victory")]
    public GameObject winPanel;
    public Text winnerText;
    public Text finalScoreText;
    public Button rematchButton;
    public Button winMenuButton;

    bool gameEnded;
    bool paused;
    bool transitionLocked;
    Coroutine serveRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(Instance.gameObject);
        Instance = this;
        Time.timeScale = 1f;
        Application.targetFrameRate = 120;
    }

    void Start()
    {
        scoreManager.ResetScore();
        WireButtons();
        if (pausePanel != null) pausePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        StartServe(0, true);
    }

    void WireButtons()
    {
        if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
        if (restartButton != null) restartButton.onClick.AddListener(RestartMatch);
        if (pauseMenuButton != null) pauseMenuButton.onClick.AddListener(BackToMenu);
        if (rematchButton != null) rematchButton.onClick.AddListener(RestartMatch);
        if (winMenuButton != null) winMenuButton.onClick.AddListener(BackToMenu);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameEnded)
            TogglePause();
    }

    void OnApplicationFocus(bool focus)
    {
        if (!focus && !gameEnded && !paused && Time.timeSinceLevelLoad > 1f)
            Pause();
    }

    public void ScorePoint(int player)
    {
        if (gameEnded || transitionLocked) return;
        transitionLocked = true;
        ball.StopBall();

        int score = scoreManager.AddPoint(player);
        AudioManager.Instance?.PlayGoal();
        CameraShake2D.Instance?.Shake(0.085f, 0.16f);

        if (roundStatusText != null)
            roundStatusText.text = player == 1 ? "PUNTO PARA PLAYER 1" : "PUNTO PARA PLAYER 2";

        if (score >= maxScore)
        {
            EndGame(player);
            return;
        }

        // Serve toward the player who conceded the point.
        int serveDirection = player == 1 ? 1 : -1;
        StartCoroutine(NextPointRoutine(serveDirection));
    }

    IEnumerator NextPointRoutine(int serveDirection)
    {
        yield return new WaitForSeconds(0.65f);
        transitionLocked = false;
        StartServe(serveDirection, false);
    }

    void StartServe(int horizontalDirection, bool firstRound)
    {
        if (serveRoutine != null) StopCoroutine(serveRoutine);
        serveRoutine = StartCoroutine(ServeRoutine(horizontalDirection, firstRound));
    }

    IEnumerator ServeRoutine(int horizontalDirection, bool firstRound)
    {
        ball.ResetBall();
        if (roundStatusText != null)
            roundStatusText.text = firstRound ? "PRIMERO EN LLEGAR A 5 GANA" : "PREPARADOS";

        string[] steps = { "3", "2", "1", "¡YA!" };
        foreach (string step in steps)
        {
            if (countdownText != null) countdownText.text = step;
            yield return new WaitForSeconds(step == "¡YA!" ? 0.32f : 0.48f);
        }

        if (countdownText != null) countdownText.text = "";
        if (roundStatusText != null) roundStatusText.text = "";
        ball.Launch(horizontalDirection);
        serveRoutine = null;
    }

    void EndGame(int winner)
    {
        gameEnded = true;
        transitionLocked = true;
        ball.StopBall();
        if (countdownText != null) countdownText.text = "";
        if (roundStatusText != null) roundStatusText.text = "";
        if (winnerText != null) winnerText.text = $"PLAYER {winner} GANA";
        if (finalScoreText != null)
            finalScoreText.text = $"{scoreManager.Player1Score}  -  {scoreManager.Player2Score}";
        if (winPanel != null) winPanel.SetActive(true);
        AudioManager.Instance?.PlayWin();
    }

    public void TogglePause()
    {
        if (paused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (gameEnded) return;
        paused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        AudioManager.Instance?.PlayClick();
    }

    public void Resume()
    {
        paused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        AudioManager.Instance?.PlayClick();
    }

    public void RestartMatch()
    {
        Time.timeScale = 1f;
        AudioManager.Instance?.PlayClick();
        SceneManager.LoadScene("Game");
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        AudioManager.Instance?.PlayClick();
        SceneManager.LoadScene("Menu");
    }
}
