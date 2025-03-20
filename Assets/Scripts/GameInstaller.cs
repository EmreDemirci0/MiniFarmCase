using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {


        Container.Bind<ResourceCollector>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ResourceManager>().FromComponentInHierarchy().AsSingle();

        Container.Bind<HayResource>().AsSingle();
        Container.Bind<FlourResource>().AsSingle();
        Container.Bind<BreadV1Resource>().AsSingle();
        Container.Bind<BreadV2Resource>().AsSingle();

        //Container.Bind<HayEntity>().FromComponentInHierarchy().AsSingle();
        //Container.Bind<FlourEntity>().FromComponentInHierarchy().AsSingle();
        //Container.Bind<BreadV1Entity>().FromComponentInHierarchy().AsSingle();
        //Container.Bind<BreadV2Entity>().FromComponentInHierarchy().AsSingle();


    }
}







