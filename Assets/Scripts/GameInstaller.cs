using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {

        //Container.Bind<ResourceCollector>().FromComponentInHierarchy().AsSingle();
        //Container.Bind<ResourceUI>().AsSingle();

        //Container.Bind<HayResource>().AsSingle();
        //Container.Bind<HayEntity>().FromComponentInHierarchy().AsSingle();

        //Container.BindFactory<ResourceBase, GeneralResourceFactory>().To<HayResource>();


        // ResourceCollector'ý baðlayalým
        Container.Bind<ResourceCollector>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ResourceManager>().FromComponentInHierarchy().AsSingle();
        // ResourceBase ve alt türlerini baðlayalým
        Container.Bind<HayResource>().AsSingle();
        Container.Bind<FlourResource>().AsSingle();

        Container.Bind<HayEntity>().FromComponentInHierarchy().AsSingle();
        Container.Bind<FlourEntity>().FromComponentInHierarchy().AsSingle();
        //Container.Bind<FlourResource>().AsSingle();

        // Fabrikayý baðlayalým
        //Container.BindFactory<ResourceBase, GeneralResourceFactory>().To<HayResource>(); // Burada HayResource türünü seçiyoruz, istediðiniz gibi deðiþtirebilirsiniz
        //Container.BindFactory<ResourceBase, GeneralResourceFactory>().To<FlourResource>();
    }
}







