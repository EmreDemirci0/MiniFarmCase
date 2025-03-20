
using UniRx;
public class BreadV2Resource : DependentResource
{
    public BreadV2Resource(ResourceManager resourceManager, ResourceCollector resourceCollector)
        : base(resourceManager, resourceCollector)
    {
        _resourceType = ResourceType.BreadV2;
        SetSubscribes();

       
    }
}
