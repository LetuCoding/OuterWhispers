using UnityEngine;

public interface IAudioManagerMenu
{

    public void PlaySFX(AudioClip clip);
    
    public void PlayMusic(AudioClip clip, bool loop = true);
    
    public void PlayRain(AudioClip clip, bool loop = true);

    public void StopRain();

    public void SetMusicVolume(float value);

    public void SetSoundVolume(float value);
    
}
