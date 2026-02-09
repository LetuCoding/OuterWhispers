using UnityEngine;
using Zenject;

public class AudioSettings : IAudioSettings
{
    private readonly AudioManagerEnemy _enemy;
    private readonly AudioManagerPlayer _player;
    private readonly AudioManagerMenu _menu;
    private readonly AudioManagerLevel _level;

    private float _sfxVolume = 1f;
    private float _musicVolume = 1f;

    [Inject]
    public AudioSettings(
        [InjectOptional] AudioManagerEnemy enemy,
        [InjectOptional] AudioManagerPlayer player,
        [InjectOptional] AudioManagerMenu menu,
        [InjectOptional] AudioManagerLevel level)
    {
        _enemy = enemy;
        _player = player;
        _menu = menu;
        _level = level;
    }

    public void SetSfxVolume(float v01)
    {
        _sfxVolume = Mathf.Clamp01(v01);

        // aplica a todos los que tengan SFX
        _enemy?.SetSoundVolume(_sfxVolume);
        _player?.SetSoundVolume(_sfxVolume);
        _menu?.SetSoundVolume(_sfxVolume);
        _level?.SetSoundVolume(_sfxVolume);
    }

    public void SetMusicVolume(float v01)
    {
        _musicVolume = Mathf.Clamp01(v01);

        // depende de cómo tengas Music en esos managers
        _menu?.SetMusicVolume(_musicVolume);
        _level?.SetMusicVolume(_musicVolume);
    }

    public void PlayEnemySfx(AudioClip clip) => _enemy?.PlaySFX(clip);
    public void PlayPlayerSfx(AudioClip clip) => _player?.PlaySFX(clip);
    public void PlayMenuSfx(AudioClip clip) => _menu?.PlaySFX(clip);
    public void PlayLevelSfx(AudioClip clip) => _level?.PlayAmbience(clip);

    public void EnemyWalkStart() => _enemy?.PlayWalk();
    public void EnemyWalkStop() => _enemy?.StopWalk();

    public void Play() { }
    
}