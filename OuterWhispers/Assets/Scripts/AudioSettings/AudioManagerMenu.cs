using UnityEngine;

public class AudioManagerMenu : IAudioManagerMenu
{
    public static AudioManagerMenu Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource rainSource;


    [Header("SFX Clips")]
    public AudioClip rain;
    public AudioClip menuMusic;
    public AudioClip introMusic;
    public AudioClip clickSound;

    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float soundVolume = 0.5f;
    
    public float minPlayTime = 5f;

    private float loopTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }

        Instance = this;
        ApplyVolumes();
    }
    private void Update()
    {
        // Contamos hacia atrás el tiempo mínimo que debe sonar
        if (loopTimer > 0f)
            loopTimer -= Time.deltaTime;
    }

    // 🔊 Reproduce un efecto
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.pitch = 1.0f;
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // 🎵 Música
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }
    
    //Lluvia
    public void PlayRain(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        rainSource.clip = clip;
        rainSource.loop = loop;
        rainSource.Play();
    }

    public void StopRain()
    {
        rainSource.Stop();
    }
    
    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        musicSource.volume = musicVolume;
    }

    public void SetSoundVolume(float value)
    {
        soundVolume = value;
        sfxSource.volume = soundVolume;
        rainSource.volume = soundVolume;

    }

    private void ApplyVolumes()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = soundVolume;
        rainSource.volume = soundVolume;
    }
    
}