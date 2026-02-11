using UnityEngine;


public interface IAudioManagerLevel
{
        public void PlayMusic(AudioClip clip, bool loop = true);

        public void PlayAmbience(AudioClip clip, bool loop = true);

        public void StopMusic();

        public void StopAmbience();

        public void SetMusicVolume(float value);

        public void SetSoundVolume(float value);

}