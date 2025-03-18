using Zenject;

public class BreadV1Entity : ResourceDependentEntity
{ 
    [Inject]
    public void Construct(BreadV1Resource breadV1Resource)
    {
        _resourceDependentBase = breadV1Resource;
        _resourceDependentBase.SetProductionValues(productionTime, maxCapacity);
        SetQueueSubscribes();
    }
}
