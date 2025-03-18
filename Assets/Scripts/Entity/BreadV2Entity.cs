
using UnityEngine;
using Zenject;

public class BreadV2Entity : ResourceDependentEntity
{
    [Inject]
    public void Construct(BreadV2Resource breadV2Resource)
    {
        Debug.Log("�u BreadV2Resource i�ine el at");
        _resourceDependentBase = breadV2Resource;
        _resourceDependentBase.SetProductionValues(productionTime, maxCapacity);
        SetQueueSubscribes();
    }
}
