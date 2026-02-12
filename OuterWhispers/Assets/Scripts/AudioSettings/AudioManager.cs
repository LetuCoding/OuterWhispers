using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : IAudioManager
{
    private float _soundVolume;
    private float _musicVolume;
    public void PlaySFX(AudioClip clip, AudioSource source, float soundPitch)
    {
        source.volume = _soundVolume;
        source.pitch = soundPitch;
        if (clip == null) return;
        source.PlayOneShot(clip);
    }
    
    public void PlayWalk(AudioClip clip, AudioSource source, float soundPitch)
    {
        source.volume = _soundVolume;
        if (!source.isPlaying)
        {
            source.clip = clip;
            source.loop = true;
            source.pitch = 0.5f;
            source.Play();
        }
    }

    public void StopWalk(AudioSource source)
    {
        if (source.isPlaying)
            source.Stop();
    }


    public void SetSoundVolume(float value, AudioMixer mixer)
    {
        
        _soundVolume = value;
        value = Mathf.Clamp(value, 0.0001f, 1f);
        float dB = Mathf.Log10(value) * 20f;
        mixer.SetFloat("SFX",dB);
    }
    public void PlayMusic(AudioClip clip, AudioSource source)
    {
        source.volume = _musicVolume;
        if (clip == null) return;
        source.clip = clip;
        source.loop = true;
        source.Play();
    }
    
    public void PlayAmbience(AudioClip clip, AudioSource source, bool loop = true)
    {
        source.volume = _soundVolume;
        if (clip == null) return;
        source.clip = clip;
        source.loop = loop;
        source.Play();
    }

    public void StopMusic(AudioSource source)
    {
        source.Stop();
    }

    public void StopAmbience(AudioSource source)
    {
        source.Stop();
    }
    
    public void SetMusicVolume(float value, AudioMixer mixer)
    {
        _musicVolume = value;
        value = Mathf.Clamp(value, 0.0001f, 1f);
        float dB = Mathf.Log10(value) * 20f;
        mixer.SetFloat("Music",dB);
    }
    public void PlayRain(AudioClip clip, AudioSource source, bool loop = true)
    {
        source.volume = _soundVolume;
        if (clip == null) return;
        source.clip = clip;
        source.loop = loop;
        source.Play();
    }

    public void StopRain(AudioSource source)
    {
        source.Stop();
    }
}
