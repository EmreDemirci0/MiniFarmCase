using Cysharp.Threading.Tasks;
using UniRx;
using System.Linq;
using System;
public abstract class ResourceBase
{
    protected DateTime _lastSavedTime;
    protected string _lastSavedTimeKey;
    protected string _storedResourceKey;

    public ResourceType ResourceType;
    protected ResourceManager _resourceManager;

    private int _maxCapacity = 5;
    public int MaxCapacity => _maxCapacity;


    private int _productionTime = 150;
    public int ProductionTime => _productionTime;


    private bool _isProducing = false;
    public bool IsProducing => _isProducing;


    private IntReactiveProperty _storedResources = new IntReactiveProperty(0);
    public IReadOnlyReactiveProperty<int> StoredResources => _storedResources;

    public bool IsSaveable;
    protected float currentRemainingTimeForSave;
    protected ResourceBase(ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
    }

    protected void SetIsProducing(bool active) 
    {
        _isProducing = active;
    }
    protected void SetStoredResources(int value)
    {
        _storedResources.Value = value;
    }
    public void SetProductionValues(int productionTime, int maxCapacity)
    {
        this._productionTime = productionTime;
        this._maxCapacity = maxCapacity;
    }
    public abstract UniTask ProduceWithSlider(float remainingTime = 0);
    public abstract UniTask<int> CollectResources();
    public virtual void SetSubscribes()
    {
        StoredResources.Subscribe(stored => _resourceManager.SetResourceCapacityText(ResourceType, stored));
        SetResourceImage();
    }
    public void SetResourceImage()
    {
        var resources = _resourceManager._allResources.FirstOrDefault(r => r.resourceType == ResourceType);
        if (resources != null)
        {
            _resourceManager.SetResourceImage(ResourceType, resources.resourceSprite);
        }
    }


    //Save 
    public abstract UniTask CalculateProductionOnLoad();
    public abstract UniTaskVoid InitializeAsync();
    public abstract UniTask StartProductionAsync();

    protected virtual void SaveDatas(string lastSavedKey, string storedKey, float curr = 0)
    {
        SaveCurrentTime(lastSavedKey, curr);
        SaveStoredResources(storedKey);
    }
    protected virtual DateTime LoadSavedTime(string lastSavedKey)
    {
        return PlayerPrefsHelper.LoadDateTime(lastSavedKey);
    }

    protected virtual void SaveStoredResources(string storedKey)
    {
        PlayerPrefsHelper.SaveInt(storedKey, StoredResources.Value);
    }
    protected virtual void LoadStoredResources(string storedKey)
    {
        int storedResources = PlayerPrefsHelper.LoadInt(storedKey);
        SetStoredResources(storedResources);
    }
    protected virtual void SaveCurrentTime(string lastSavedKey, float curr)
    {
        _lastSavedTime = DateTime.Now - TimeSpan.FromSeconds(curr > 0 ? ProductionTime - curr : 0);
        PlayerPrefsHelper.SaveDateTime(lastSavedKey, _lastSavedTime);
    }
}