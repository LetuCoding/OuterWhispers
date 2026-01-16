using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("SFX Clips")]
    public AudioClip footstep;
    public AudioClip dash;
    public AudioClip jump;
    public AudioClip slide;
    public AudioClip falling;

    public float minPlayTime = 5f;

    private float loopTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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
}
