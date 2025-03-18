using Zenject;
public class FlourEntity : ResourceDependentEntity 
{
    [Inject]
    public void Construct(FlourResource flourResource)
    {
        _resourceDependentBase = flourResource;
        _resourceDependentBase.SetProductionValues(productionTime, maxCapacity);
        SetQueueSubscribes();
    }
}
