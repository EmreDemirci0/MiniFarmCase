using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class ResourceManager : MonoBehaviour
{
    public List<SCResources> allResources;
    public ReactiveProperty<int> TotalHayCount { get; private set; } = new ReactiveProperty<int>(100);
    public ReactiveProperty<int> TotalFlourCount { get; private set; } = new ReactiveProperty<int>(100);
    public ReactiveProperty<int> TotalBreadCount { get; private set; } = new ReactiveProperty<int>(0);

    [SerializeField] private TextMeshProUGUI totalHayCountText;
    [SerializeField] private TextMeshProUGUI totalFlourCountText;
    [SerializeField] private TextMeshProUGUI totalBreadCountText;

    private void Start()
    {
        TotalHayCount.Subscribe(count => SetTotalText(ResourceType.Hay, count)).AddTo(this);
        TotalFlourCount.Subscribe(count => SetTotalText(ResourceType.Flour, count)).AddTo(this);
        TotalBreadCount.Subscribe(count => SetTotalText(ResourceType.BreadV1, count)).AddTo(this);
        TotalBreadCount.Subscribe(count => SetTotalText(ResourceType.BreadV2, count)).AddTo(this);
    }
    public void SetTotalText(ResourceType resourceType, int count)
    {
        switch (resourceType)
        {
            case ResourceType.Hay:
                totalHayCountText.text = count.ToString();
                break;
            case ResourceType.Flour:
                totalFlourCountText.text = count.ToString();
                break;
            case ResourceType.BreadV1:
            case ResourceType.BreadV2:
                totalBreadCountText.text = count.ToString();
                break;
            default:
                Debug.LogWarning("Unknown ResourceType");
                break;
        }
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

    public void RemoveResource(ResourceType resourceType, int quantity)
    {
        switch (resourceType)
        {
            case ResourceType.Hay:
                if (TotalHayCount.Value >= quantity)
                {
                    TotalHayCount.Value -= quantity;
                }
                break;
            case ResourceType.Flour:
                if (TotalFlourCount.Value >= quantity)
                {
                    TotalFlourCount.Value -= quantity;
                }
                break;
            case ResourceType.BreadV1:
                if (TotalBreadCount.Value >= quantity)
                {
                    TotalBreadCount.Value -= quantity;
                }
                break;
            case ResourceType.BreadV2:
                if (TotalBreadCount.Value >= quantity)
                {
                    TotalBreadCount.Value -= quantity;
                }
                break;
        }
    }
}
