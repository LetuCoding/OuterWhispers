using UnityEngine;


public interface IAudioManagerEnemy
{ 
        public void PlaySFX(AudioClip clip);

        public void PlayWalk();

        public void StopWalk();
        
        public void SetSoundVolume(float value);

}