using Cysharp.Threading.Tasks;
using UniRx;

public abstract class ResourceBase
{
    protected ResourceManager _resourceManager;
    protected ResourceCollector _resourceCollector;
    protected ResourceType resourceType;

    private int maxCapacity = 5; // Fabrikanýn kapasitesi
    public int MaxCapacity => maxCapacity;

    private int productionTime = 150; // Üretim süresi (saniye)
    public int ProductionTime => productionTime;

    public void SetProductionValues(int productionTime, int maxCapacity)
    {
        this.productionTime = productionTime;
        this.maxCapacity = maxCapacity;
    }

    private bool isProducing = false;
    public bool IsProducing => isProducing;
    protected void SetIsProducing(bool active)
    {
        isProducing = active;
    }

    private IntReactiveProperty storedResources = new IntReactiveProperty(0);
    public IReadOnlyReactiveProperty<int> StoredResources => storedResources;
    protected void SetStoredResources(int value)
    {
        storedResources.Value = value;
    }
    protected ResourceBase(ResourceManager resourceManager, ResourceCollector resourceCollector, ResourceType resourceType)
    {
        _resourceManager = resourceManager;
        _resourceCollector = resourceCollector;
        this.resourceType = resourceType;

        StoredResources.Subscribe(stored => _resourceCollector.SetStoredResourcesText(resourceType, stored));
    }
    public abstract UniTask Produce(); 
    public abstract UniTask<int> CollectResources();
}