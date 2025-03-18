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

    protected IntReactiveProperty storedResources = new IntReactiveProperty(0); // Depodaki kaynak say�s�
    private ReactiveProperty<int> productionTimeLeft = new ReactiveProperty<int>();

    protected int productionTime = 150; // �retim s�resi (saniye)
    protected int maxCapacity = 5; // Fabrikan�n kapasitesi


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

        // StoredResources de�i�ti�inde UI'yi g�ncelle
        StoredResources.Subscribe(stored => _resourceCollector.SetStoredResourcesText(resourceType, stored));
    }
    public abstract UniTask Produce(); // Her kaynak t�r� farkl� �retim yapacak.
    public abstract UniTask<int> CollectResources(); // Her kaynak t�r� farkl� �retim yapacak.

    public void SetProductionValues(int productionTime, int maxCapacity)
    {
        
        this.productionTime = productionTime;
        this.maxCapacity = maxCapacity;
    }

}