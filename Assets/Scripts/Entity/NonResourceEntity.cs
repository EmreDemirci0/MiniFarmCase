using Cysharp.Threading.Tasks;
using Zenject;

public abstract class NonResourceEntity<TResource> : EntityBase where TResource : NonResource
{
    protected TResource _resourceNon;

    [Inject]
    public void ConstructBase(TResource resource)
    {
        _resourceNon = resource;
        _resourceNon.ResourceType = GetResourceType();
        _resourceNon.SetSubscribes();
        _resourceNon.SetProductionValues(resourceInfo.productionTime, resourceInfo.maxCapacity);
        
    }
    public async override UniTaskVoid Interact()
    {
        if (_resourceNon != null)
        {
            await _resourceNon.CollectResources();
        }
    }
}