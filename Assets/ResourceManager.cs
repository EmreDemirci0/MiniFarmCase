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

        TotalHayCount.Subscribe(hayCount => _resourceCollector.SetTotalHayText(hayCount)).AddTo(this);
        TotalFlourCount.Subscribe(flourCount => _resourceCollector.SetTotalFlourText(flourCount)).AddTo(this);
        TotalBreadCount.Subscribe(flourCount => _resourceCollector.SetTotalBreadText(flourCount)).AddTo(this);
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
            default:
                return 0;
        }
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
        }
        return false;
    }
}
