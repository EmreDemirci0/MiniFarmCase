using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {


        // ResourceCollector'ý baðlayalým
        Container.Bind<ResourceCollector>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ResourceManager>().FromComponentInHierarchy().AsSingle();
        // ResourceBase ve alt türlerini baðlayalým
        Container.Bind<HayResource>().AsSingle();
        Container.Bind<FlourResource>().AsSingle();
        Container.Bind<BreadV1Resource>().AsSingle();

        Container.Bind<HayEntity>().FromComponentInHierarchy().AsSingle();
        Container.Bind<FlourEntity>().FromComponentInHierarchy().AsSingle();
        Container.Bind<BreadV1Entity>().FromComponentInHierarchy().AsSingle();
      
    }
}







