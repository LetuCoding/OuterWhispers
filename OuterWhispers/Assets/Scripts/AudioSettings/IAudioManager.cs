using UnityEngine;
using UnityEngine.Audio;


public interface IAudioManager
{
        public void PlaySFX(AudioClip clip, AudioSource source, float soundPitch);

        public void PlayWalk(AudioClip clip, AudioSource source, float soundPitch);

        public void StopWalk(AudioSource source);


        public void SetSoundVolume(float value, AudioMixer mixer);

        public void PlayMusic(AudioClip clip, AudioSource source);

        public void PlayAmbience(AudioClip clip, AudioSource source, bool loop = true);

        public void StopMusic(AudioSource source);

        public void StopAmbience(AudioSource source);

        public void SetMusicVolume(float value, AudioMixer mixer);

        public void PlayRain(AudioClip clip, AudioSource source, bool loop = true);
        
        public void StopRain(AudioSource source);

}