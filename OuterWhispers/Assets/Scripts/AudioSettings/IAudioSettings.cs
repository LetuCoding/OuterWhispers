using UnityEngine;

public interface IAudioSettings
{
    void SetSfxVolume(float v01);
    void SetMusicVolume(float v01);

    // SFX genérico
    void PlayEnemySfx(AudioClip clip);
    void PlayPlayerSfx(AudioClip clip);
    void PlayMenuSfx(AudioClip clip);
    void PlayLevelSfx(AudioClip clip);

    // Walk específico (porque tu manager lo tiene)
    void EnemyWalkStart();
    void EnemyWalkStop();

    public void Play();
}
