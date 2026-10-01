using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX")]
    public AudioClip bounceClip;
    public AudioClip wallClip;
    public AudioClip goalClip;
    public AudioClip clickClip;
    public AudioClip winClip;

    [Header("Music")]
    public AudioClip ambientLoop;
    [Range(0f, 1f)] public float musicVolume = 0.18f;
    [Range(0f, 1f)] public float sfxVolume = 0.85f;

    AudioSource sfxSource;
    AudioSource musicSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        AudioSource[] sources = GetComponents<AudioSource>();
        sfxSource = sources.Length > 0 ? sources[0] : gameObject.AddComponent<AudioSource>();
        musicSource = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();

        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        if (ambientLoop != null)
        {
            musicSource.clip = ambientLoop;
            musicSource.Play();
        }
    }

    void Play(AudioClip clip, float volume, float minPitch = 1f, float maxPitch = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume * sfxVolume));
    }

    public void PlayBounce(float intensity = 0.7f) => Play(bounceClip, Mathf.Lerp(0.55f, 1f, intensity), 0.96f, 1.06f);
    public void PlayWall() => Play(wallClip, 0.55f, 0.98f, 1.04f);
    public void PlayGoal() => Play(goalClip, 0.9f, 0.98f, 1.02f);
    public void PlayClick() => Play(clickClip, 0.55f, 1f, 1f);
    public void PlayWin() => Play(winClip, 1f, 1f, 1f);
}
