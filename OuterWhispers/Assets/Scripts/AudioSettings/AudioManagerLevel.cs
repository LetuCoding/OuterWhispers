using UnityEngine;
using Zenject;

public class AudioManagerLevel : IAudioManagerLevel
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;
    
    [Header("SFX Clips")]
    public AudioClip musicAmbience;
    public AudioClip musicBoss;
    public AudioClip ambienceSound;

    [Range(0f, 1f)] public float musicVolume = 0.5f;
    
    public float minPlayTime = 5f;

    private float loopTimer;

    private void Awake()
    {
        ApplyVolumes();
        PlayMusic(musicAmbience);
        PlayAmbience(ambienceSound);
    }
    private void Update()
    {
        // Contamos hacia atrás el tiempo mínimo que debe sonar
        if (loopTimer > 0f)
            loopTimer -= Time.deltaTime;
    }

    // 🎵 Música
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }
    
    public void PlayAmbience(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        ambienceSource.clip = clip;
        ambienceSource.loop = loop;
        ambienceSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void StopAmbience()
    {
        ambienceSource.Stop();
    }
    
    public void SetMusicVolume(float value)
    {
        musicSource.volume = value;
    }
    
    public void SetSoundVolume(float value)
    {
        ambienceSource.volume = value;
    }
    
    private void ApplyVolumes()
    {
        musicSource.volume = musicVolume;
    }
}
