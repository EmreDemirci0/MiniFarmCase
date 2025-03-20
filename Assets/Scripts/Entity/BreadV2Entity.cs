
using UnityEngine;
using Zenject;

public class BreadV2Entity : ResourceDependentEntity
{
    [Inject]
    public void Construct(BreadV2Resource breadV2Resource)
    {
        _resourceDependentBase = breadV2Resource;
        _resourceDependentBase.SetProductionValues(resourceInfo.productionTime, resourceInfo.maxCapacity);
        SetQueueSubscribes();
    }
}
