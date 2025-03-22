using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using System.Linq;
using System;

public abstract class ResourceBase 
{
    protected DateTime lastSavedTime;


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

    public bool IsSaveable;
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
    //public abstract UniTask Produce(); 
    public abstract UniTask ProduceWithSlider(float remainingTime = 0);
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

    //Save
    protected virtual async UniTask CalculateProductionOnLoad()//Eðer kayýtlý bir zaman var ise, kaldýgý yerden devam ettirir
    {
        if (lastSavedTime != default)
        {
            TimeSpan timeDifference = DateTime.Now - lastSavedTime;


            int producedResources = (int)(timeDifference.TotalSeconds / ProductionTime);
            var remainingResources = (timeDifference.TotalSeconds % ProductionTime);



            int newStoredResources = Mathf.Min(MaxCapacity, StoredResources.Value + producedResources);
            SetStoredResources(newStoredResources);

            if (newStoredResources < MaxCapacity)
            {

                double remainingTime = ProductionTime - remainingResources;
                _resourceCollector.SetSliderValue(ResourceType, (float)(remainingTime / ProductionTime));
                await ProduceWithSlider((float)(remainingTime)); // Üretimi kaldýgý yerden devam ettir
            }
            else
            {
                _resourceCollector.SetSliderValue(ResourceType, 1); // Depo dolu, slider tam dolu
                _resourceCollector.SetProductionTimerText(ResourceType, "FULL");
            }
        }
    }
    protected virtual void SAVE(string lastSavedKey,string storedKey,float curr = 0)
    {
        
        //Debug.Log("SAVED");
        SaveCurrentTime(lastSavedKey , curr);
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
        SetStoredResources(storedResources); // Depo durumunu yükle
    }
    protected virtual void SaveCurrentTime(string lastSavedKey,float curr)
    {
        if (curr > 0)
        {
            lastSavedTime = DateTime.Now - TimeSpan.FromSeconds(ProductionTime - curr);
        }
        else
        {
            lastSavedTime = DateTime.Now; // curr deðeri yoksa þu anki zamaný kaydet
        }
        PlayerPrefsHelper.SaveDateTime(lastSavedKey, lastSavedTime);
        //Debug.Log("Zaman kaydedildi: " + lastSavedTime);
    }
    
}