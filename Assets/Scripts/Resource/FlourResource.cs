using Zenject.ReflectionBaking.Mono.Cecil;
using UniRx;

public class FlourResource : DependentResource
{
    public FlourResource(ResourceManager resourceManager, ResourceCollector resourceCollector ) 
        : base(resourceManager, resourceCollector)
    {
        _resourceType = ResourceType.Flour;
       
        SetSubscribes();
    }
}
