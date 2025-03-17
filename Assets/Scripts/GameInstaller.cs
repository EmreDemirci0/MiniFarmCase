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


        // ResourceCollector'� ba�layal�m
        Container.Bind<ResourceCollector>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ResourceManager>().FromComponentInHierarchy().AsSingle();
        // ResourceBase ve alt t�rlerini ba�layal�m
        Container.Bind<HayResource>().AsSingle();
        Container.Bind<FlourResource>().AsSingle();

        Container.Bind<HayEntity>().FromComponentInHierarchy().AsSingle();
        Container.Bind<FlourEntity>().FromComponentInHierarchy().AsSingle();
        //Container.Bind<FlourResource>().AsSingle();

        // Fabrikay� ba�layal�m
        //Container.BindFactory<ResourceBase, GeneralResourceFactory>().To<HayResource>(); // Burada HayResource t�r�n� se�iyoruz, istedi�iniz gibi de�i�tirebilirsiniz
        //Container.BindFactory<ResourceBase, GeneralResourceFactory>().To<FlourResource>();
    }
}







