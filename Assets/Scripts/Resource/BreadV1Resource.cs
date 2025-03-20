using Zenject.ReflectionBaking.Mono.Cecil;
using UniRx;
using UnityEngine;

public class BreadV1Resource : DependentResource
{
    public BreadV1Resource(ResourceManager resourceManager, ResourceCollector resourceCollector)
        : base(resourceManager, resourceCollector)
    {
        Debug.Log("hem reosurce hem entity 2 tane olmasýn. birini kýsalým bari");
        _resourceType = ResourceType.BreadV1;
        SetSubscribes();
    }
}
