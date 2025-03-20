using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using System.Linq;

public abstract class ResourceBase 
{
    protected ResourceManager _resourceManager;
    protected ResourceCollector _resourceCollector;
    protected ResourceType _resourceType;

    private int maxCapacity = 5; // Fabrikanýn kapasitesi
    public int MaxCapacity => maxCapacity;

    private int productionTime = 150; // Üretim süresi (saniye)
    public int ProductionTime => productionTime;

   

    private bool isProducing = false;
    public bool IsProducing => isProducing;
    

    private IntReactiveProperty storedResources = new IntReactiveProperty(0);
    public IReadOnlyReactiveProperty<int> StoredResources => storedResources;
    

    protected ResourceBase(ResourceManager resourceManager,ResourceCollector resourceCollector)
    {
        _resourceManager = resourceManager;
        _resourceCollector = resourceCollector;
        //this.resourceType = _entityBase.resourceInfo.resourceType;

        
        //SetStoredResources(StoredResources.Value);
    }

    protected void SetIsProducing(bool active) //setter
    {
        isProducing = active;
    }
    protected void SetStoredResources(int value) //setter
    {
        storedResources.Value = value;
        //_resourceCollector.SetResourceCapacityText(_resourceType,StoredResources.Value);
    }
    public void SetProductionValues(int productionTime, int maxCapacity)
    {
        this.productionTime = productionTime;
        this.maxCapacity = maxCapacity;
    }
    public abstract UniTask Produce(); 
    public abstract UniTask<int> CollectResources();
    public abstract void SetSubscribes();
    public void SetResourceImage()
    {
        var reso = _resourceManager.reso.FirstOrDefault(r => r.resourceType == _resourceType);
        if (reso != null)
        {
            _resourceCollector.SetResourceImage(_resourceType, reso.resourceSprite);
        }
    }
}