using Zenject;


public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IAudioSettings>().To<AudioSettings>().AsSingle();
        
        Container.Bind<AudioManagerEnemy>().FromComponentInHierarchy().AsCached();
        Container.Bind<AudioManagerPlayer>().FromComponentInHierarchy().AsCached();
        Container.Bind<AudioManagerMenu>().FromComponentInHierarchy().AsCached();
        Container.Bind<AudioManagerLevel>().FromComponentInHierarchy().AsCached();
    }
}