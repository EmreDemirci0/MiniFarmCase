using Zenject;
public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ResourceManager>().FromComponentInHierarchy().AsSingle();

        Container.Bind<HayResource>().AsSingle();
        Container.Bind<FlourResource>().AsSingle();
        Container.Bind<BreadV1Resource>().AsSingle();
        Container.Bind<BreadV2Resource>().AsSingle();
    }
}







