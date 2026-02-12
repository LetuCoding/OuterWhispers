using Zenject;


public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        
        Container.Bind<IAudioManager>().To<AudioManager>().AsSingle();

        Container.Bind<IAudioManagerPlayer>().To<AudioManagerPlayer>().AsSingle();
        
        Container.Bind<IAudioManagerMenu>().To<AudioManagerMenu>().AsSingle();
        
        Container.Bind<IAudioManagerLevel>().To<AudioManagerLevel>().AsSingle();

    }
}