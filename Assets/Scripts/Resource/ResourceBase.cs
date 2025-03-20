using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using System.Linq;

public abstract class ResourceBase 
{
    public ResourceType ResourceType;
    protected ResourceManager _resourceManager;
    protected ResourceCollector _resourceCollector;   

    private int _maxCapacity = 5; // Fabrikanýn kapasitesi
    public int MaxCapacity => _maxCapacity;


    private int _productionTime = 150; // Üretim süresi (saniye)
    public int ProductionTime => _productionTime;

   
    private bool _isProducing = false;
    public bool IsProducing => _isProducing;
    

    private IntReactiveProperty _storedResources = new IntReactiveProperty(0);
    public IReadOnlyReactiveProperty<int> StoredResources => _storedResources;
    

    protected ResourceBase(ResourceManager resourceManager,ResourceCollector resourceCollector)
    {
        _resourceManager = resourceManager;
        _resourceCollector = resourceCollector;
    }

    protected void SetIsProducing(bool active) //setter
    {
        _isProducing = active;
    }
    protected void SetStoredResources(int value) //setter
    {
        _storedResources.Value = value;
        //_resourceCollector.SetResourceCapacityText(_resourceType,StoredResources.Value);
    }
    public void SetProductionValues(int productionTime, int maxCapacity)
    {
        this._productionTime = productionTime;
        this._maxCapacity = maxCapacity;
    }
    public abstract UniTask Produce(); 
    public abstract UniTask<int> CollectResources();
    public abstract void SetSubscribes();
    public void SetResourceImage()
    {
        var resources = _resourceManager.allResources.FirstOrDefault(r => r.resourceType == ResourceType);
        if (resources != null)
        {
            _resourceCollector.SetResourceImage(ResourceType, resources.resourceSprite);
        }
    }
}