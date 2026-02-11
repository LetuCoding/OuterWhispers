using UnityEngine;

public interface IAudioManagerPlayer
{

    public void PlaySFX(AudioClip clip);

    public void PlayWalk();

    public void StopWalk();

    public void SetSoundVolume(float value);
    
}
