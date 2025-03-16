using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ResourceFactory>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ResourceUIController>().FromComponentInHierarchy().AsSingle();

    }
}
