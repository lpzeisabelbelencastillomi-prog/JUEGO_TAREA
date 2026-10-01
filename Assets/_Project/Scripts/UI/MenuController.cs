using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject controlsPanel;

    void Start()
    {
        Bind("PlayButton", PlayGame);
        Bind("ControlsButton", ShowControls);
        Bind("ExitButton", ExitGame);
        Bind("ControlsBackButton", HideControls);
    }

    void Bind(string name, UnityEngine.Events.UnityAction action)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) return;
        Button b = go.GetComponent<Button>();
        if (b != null) b.onClick.AddListener(() => { AudioManager.Instance?.PlayClick(); action(); });
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
