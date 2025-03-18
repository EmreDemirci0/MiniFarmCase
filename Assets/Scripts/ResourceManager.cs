using System;
using UniRx;
using UnityEngine;
using Zenject;

public class ResourceManager : MonoBehaviour
{
    private ResourceCollector _resourceCollector;

    public ReactiveProperty<int> TotalHayCount { get; private set; } = new ReactiveProperty<int>(100);
    public ReactiveProperty<int> TotalFlourCount { get; private set; } = new ReactiveProperty<int>(100);
    public ReactiveProperty<int> TotalBreadCount { get; private set; } = new ReactiveProperty<int>(0);


    [Inject]
    public void Construct(ResourceCollector resourceCollector)
    {
        _resourceCollector = resourceCollector;

        TotalHayCount.Subscribe(count => _resourceCollector.SetTotalText(ResourceType.Hay, count)).AddTo(this);
        TotalFlourCount.Subscribe(count => _resourceCollector.SetTotalText(ResourceType.Flour, count)).AddTo(this);
        TotalBreadCount.Subscribe(count => _resourceCollector.SetTotalText(ResourceType.BreadV1, count)).AddTo(this);
        TotalBreadCount.Subscribe(count => _resourceCollector.SetTotalText(ResourceType.BreadV2, count)).AddTo(this);
    }
   
    public int GetTotalResourceCount(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Hay:
                return TotalHayCount.Value;
            case ResourceType.Flour:
                return TotalFlourCount.Value;
            case ResourceType.BreadV1:
                return TotalBreadCount.Value;
            case ResourceType.BreadV2:
                return TotalBreadCount.Value;
            default:
                return 0;
        }
    }
    public IObservable<int> GetTotalResourceObservable(ResourceType resourceType)
    {
        return resourceType switch
        {
            ResourceType.Hay => TotalHayCount,
            ResourceType.Flour => TotalFlourCount,
            ResourceType.BreadV1 => TotalBreadCount,
            ResourceType.BreadV2 => TotalBreadCount,
            _ => Observable.Return(0)
        };
    }


    public void AddResource(ResourceType resourceType, int quantity)
    {
        switch (resourceType)
        {
            case ResourceType.Hay:
                TotalHayCount.Value += quantity;
                break;
            case ResourceType.Flour:
                TotalFlourCount.Value += quantity;
                break;
            case ResourceType.BreadV1:
                TotalBreadCount.Value += quantity;
                break;
            case ResourceType.BreadV2:
                TotalBreadCount.Value += quantity;
                break;
        }
    }

    public bool RemoveResource(ResourceType resourceType, int quantity)
    {
        switch (resourceType)
        {
            case ResourceType.Hay:
                if (TotalHayCount.Value >= quantity)
                {
                    TotalHayCount.Value -= quantity;
                    return true;
                }
                break;
            case ResourceType.Flour:
                if (TotalFlourCount.Value >= quantity)
                {
                    TotalFlourCount.Value -= quantity;
                    return true;
                }
                break;
            case ResourceType.BreadV1:
                if (TotalBreadCount.Value >= quantity)
                {
                    TotalBreadCount.Value -= quantity;
                    return true;
                }
                break;
            case ResourceType.BreadV2:
                if (TotalBreadCount.Value >= quantity)
                {
                    TotalBreadCount.Value -= quantity;
                    return true;
                }
                break;
        }
        return false;
    }
}
