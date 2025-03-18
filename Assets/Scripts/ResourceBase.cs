using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class ResourceBase
{
    public abstract UniTask Produce(); // Her kaynak t�r� farkl� �retim yapacak.
    public abstract UniTask<int> CollectResources(); // Her kaynak t�r� farkl� �retim yapacak.

    protected ResourceType resourceType;

    protected IntReactiveProperty storedResources = new IntReactiveProperty(0); // Depodaki kaynak say�s�
    private ReactiveProperty<int> productionTimeLeft = new ReactiveProperty<int>();

    protected int productionTime = 5; // �retim s�resi (saniye)
    protected int maxCapacity = 5; // Fabrikan�n kapasitesi


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