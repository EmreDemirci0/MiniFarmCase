using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

public abstract class ResourceBase
{
    protected ResourceManager _resourceManager;
    protected ResourceType resourceType;
    protected EntityBase _entityBase;

    private int maxCapacity = 5; // Fabrikanýn kapasitesi
    public int MaxCapacity => maxCapacity;

    private int productionTime = 150; // Üretim süresi (saniye)
    public int ProductionTime => productionTime;

   

    private bool isProducing = false;
    public bool IsProducing => isProducing;
    

    private IntReactiveProperty storedResources = new IntReactiveProperty(0);
    public IReadOnlyReactiveProperty<int> StoredResources => storedResources;
    

    protected ResourceBase(ResourceManager resourceManager, EntityBase entityBase)
    {
        _resourceManager = resourceManager;
        _entityBase = entityBase;
        this.resourceType = _entityBase.resourceInfo.resourceType;

        StoredResources.Subscribe(stored => _entityBase.SetResourceCapacityText(stored));
        SetStoredResources(StoredResources.Value);
    }

    protected void SetIsProducing(bool active) //setter
    {
        isProducing = active;
    }
    protected void SetStoredResources(int value) //setter
    {
        storedResources.Value = value;
        //_entityBase.SetResourceCapacityText(StoredResources.Value);
    }
    public void SetProductionValues(int productionTime, int maxCapacity)
    {
        this.productionTime = productionTime;
        this.maxCapacity = maxCapacity;
    }
    public abstract UniTask Produce(); 
    public abstract UniTask<int> CollectResources();
}