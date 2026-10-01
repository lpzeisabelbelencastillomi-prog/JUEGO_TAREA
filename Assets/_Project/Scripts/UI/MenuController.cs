using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject controlsPanel;
    public Button playButton;
    public Button controlsButton;
    public Button exitButton;
    public Button controlsBackButton;

    void Start()
    {
        if (playButton != null) playButton.onClick.AddListener(() => { AudioManager.Instance?.PlayClick(); PlayGame(); });
        if (controlsButton != null) controlsButton.onClick.AddListener(() => { AudioManager.Instance?.PlayClick(); ShowControls(); });
        if (exitButton != null) exitButton.onClick.AddListener(() => { AudioManager.Instance?.PlayClick(); ExitGame(); });
        if (controlsBackButton != null) controlsBackButton.onClick.AddListener(() => { AudioManager.Instance?.PlayClick(); HideControls(); });
    }

    public void PlayGame() => SceneManager.LoadScene("Game");
    public void ShowControls() { if (controlsPanel != null) controlsPanel.SetActive(true); }
    public void HideControls() { if (controlsPanel != null) controlsPanel.SetActive(false); }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
