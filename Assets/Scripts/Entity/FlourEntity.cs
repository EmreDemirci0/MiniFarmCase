using Zenject;
public class FlourEntity : ResourceDependentEntity 
{
    [Inject]
    public void Construct(FlourResource flourResource)
    {
        _resourceDependentBase = flourResource;
        _resourceDependentBase.SetProductionValues(resourceInfo.productionTime, resourceInfo.maxCapacity);
        SetQueueSubscribes();
    }
}
