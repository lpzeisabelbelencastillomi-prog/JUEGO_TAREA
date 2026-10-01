using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioClip bounceClip;
    public AudioClip wallClip;
    public AudioClip goalClip;
    public AudioClip clickClip;
    public AudioClip winClip;
    private AudioSource source;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        source = gameObject.GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
    }

    void Play(AudioClip clip, float volume = 1f)
    {
        if (clip != null && source != null) source.PlayOneShot(clip, volume);
    }

    public void PlayBounce() => Play(bounceClip, 0.8f);
    public void PlayWall() => Play(wallClip, 0.6f);
    public void PlayGoal() => Play(goalClip, 0.9f);
    public void PlayClick() => Play(clickClip, 0.7f);
    public void PlayWin() => Play(winClip, 0.9f);
}
