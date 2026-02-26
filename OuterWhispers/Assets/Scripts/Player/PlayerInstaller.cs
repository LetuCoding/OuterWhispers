using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    //Binding player in Project Context
    public override void InstallBindings()
    {
        Container.Bind<Player>()
            .FromComponentInHierarchy()
            .AsSingle();
    }
}
