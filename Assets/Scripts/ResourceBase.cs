using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class ResourceBase
{
    public abstract UniTask Produce(); // Her kaynak türü farklý üretim yapacak.
    public abstract UniTask<int> CollectResources(); // Her kaynak türü farklý üretim yapacak.

    protected ResourceType resourceType;

    protected IntReactiveProperty storedResources = new IntReactiveProperty(0); // Depodaki kaynak sayýsý
    private ReactiveProperty<int> productionTimeLeft = new ReactiveProperty<int>();

    protected int productionTime = 5; // Üretim süresi (saniye)
    protected int maxCapacity = 5; // Fabrikanýn kapasitesi


    protected bool isProducing = false;

    public IReadOnlyReactiveProperty<int> StoredResources => storedResources;
    public IReadOnlyReactiveProperty<int> ProductionTimeLeft => productionTimeLeft;

    public int MaxCapacity => maxCapacity;
    public int ProductionTime => productionTime;



    public void SetProductionValues(int productionTime, int maxCapacity)
    {
        this.productionTime = productionTime;
        this.maxCapacity = maxCapacity;
    }

}