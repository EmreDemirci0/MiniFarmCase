using Zenject.ReflectionBaking.Mono.Cecil;
using UniRx;

public class HayResource : NonResource
{
    public HayResource(ResourceManager resourceManager, ResourceCollector resourceCollector)
        : base(resourceManager, resourceCollector)
    {
        _resourceType = ResourceType.Hay;
        SetSubscribes();
    }
}