using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public abstract class ResourceBase
{
   

    protected ResourceManager _resourceManager;
    protected ResourceCollector _resourceCollector;
    protected ResourceType resourceType;

    protected IntReactiveProperty storedResources = new IntReactiveProperty(0); // Depodaki kaynak sayýsý
    private ReactiveProperty<int> productionTimeLeft = new ReactiveProperty<int>();

    protected int productionTime = 150; // Üretim süresi (saniye)
    protected int maxCapacity = 5; // Fabrikanýn kapasitesi


    protected bool isProducing = false;

    public IReadOnlyReactiveProperty<int> StoredResources => storedResources;
    public IReadOnlyReactiveProperty<int> ProductionTimeLeft => productionTimeLeft;

    public int MaxCapacity => maxCapacity;
    public int ProductionTime => productionTime;

    //[Inject]
    protected ResourceBase(ResourceManager resourceManager, ResourceCollector resourceCollector, ResourceType resourceType)
    {
        _resourceManager = resourceManager;
        _resourceCollector = resourceCollector;
        this.resourceType = resourceType;

        // StoredResources deðiþtiðinde UI'yi güncelle
        StoredResources.Subscribe(stored => _resourceCollector.SetStoredResourcesText(resourceType, stored));
    }
    public abstract UniTask Produce(); // Her kaynak türü farklý üretim yapacak.
    public abstract UniTask<int> CollectResources(); // Her kaynak türü farklý üretim yapacak.

    public void SetProductionValues(int productionTime, int maxCapacity)
    {
        
        this.productionTime = productionTime;
        this.maxCapacity = maxCapacity;
    }

}