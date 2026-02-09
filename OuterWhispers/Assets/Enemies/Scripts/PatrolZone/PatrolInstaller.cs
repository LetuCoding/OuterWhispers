using UnityEngine;
using Zenject;
namespace Enemies.Scripts
{
    public class PatrolInstaller : MonoInstaller
    {
        
        
        public override void InstallBindings()
        {
            Container.Bind<IPatrolZoneService>()
                .To<PatrolZoneService>()
                .AsSingle();
        }
    }
}