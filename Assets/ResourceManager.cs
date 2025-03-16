using UniRx;
using UnityEngine;
using Zenject;

public class ResourceManager : MonoBehaviour
{
    // ReactiveProperty kullanarak bu deðerleri dinamik yapalým
    public ReactiveProperty<int> TotalHayCount { get; private set; } = new ReactiveProperty<int>(10);
    public ReactiveProperty<int> TotalFlourCount { get; private set; } = new ReactiveProperty<int>(0);

    [Inject]
    private void Construct()
    {
        // Eðer baþka baðýmlýlýk varsa burada alabiliriz.
    }

    public int GetTotalResourceCount(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Hay:
                return TotalHayCount.Value;
            case ResourceType.Flour:
                return TotalFlourCount.Value;
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
        }
        return false;
    }
}
