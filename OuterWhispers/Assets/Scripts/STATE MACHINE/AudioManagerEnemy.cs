using UnityEngine;

public class AudioManagerEnemy : MonoBehaviour
{
    public static AudioManagerEnemy Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;

    [Header("SFX Clips")]
    public AudioClip footstep;
    public AudioClip shoot;
    public AudioClip damage;
    public AudioClip dead;
    public AudioClip chains;
    public AudioClip smash;

    [Range(0f, 1f)] public float soundVolume = 0.5f;
    
    public float soundPitch = 1f;

    private float loopTimer;

    private void Awake()
    {
     
    }
    private void Update()
    {
        // Contamos hacia atrás el tiempo mínimo que debe sonar
        if (loopTimer > 0f)
            loopTimer -= Time.deltaTime;

        // Si NO queremos caminar y ya pasó el mínimo, paramos el loop
        if (!footstep && loopTimer <= 0f && sfxSource.isPlaying)
        {
            sfxSource.Stop();
        }
    }

    // 🔊 Reproduce un efecto
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.pitch = soundPitch;
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
    
    public void PlayWalk()
    {
            if (!sfxSource.isPlaying)
        {
            sfxSource.loop = true;
            sfxSource.pitch = 0.5f;
            sfxSource.Play();
        }
    }

    public void StopWalk()
    {
        if (sfxSource.isPlaying)
            sfxSource.Stop();
    }


    public void SetSoundVolume(float value)
    {
        soundVolume = value;
        sfxSource.volume = soundVolume;

    }

    private void ApplyVolumes()
    {
        sfxSource.volume = soundVolume;
    }
}
