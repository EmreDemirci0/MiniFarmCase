using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

public class DependentResource : ResourceBase
{
    private ReactiveProperty<int> _queueCount = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> QueueCount => _queueCount;
    //protected string queueCountKey;
    public DependentResource(ResourceManager resourceManager, ResourceCollector resourceCollector)
      : base(resourceManager, resourceCollector)
    {
        InitializeAsync().Forget();
    }

    public override async UniTaskVoid InitializeAsync()
    {
        await UniTask.DelayFrame(1);

        if (IsSaveable)
        {
            _lastSavedTime = LoadSavedTime(ConstantKeys.FlourLastSavedTimeKey);
            LoadStoredResources(ConstantKeys.FlourStoredResourceKey);
            LoadQueueCount(ConstantKeys.FlourQueueCountKey);

            if (_lastSavedTime == default)
            {
                Debug.Log("Kaydedilen zaman bulunamad�. Yeni �retim ba�lat�l�yor.");
                SaveDatas(ConstantKeys.FlourLastSavedTimeKey, ConstantKeys.FlourStoredResourceKey);
            }
        }

        await StartProductionAsync();
    }
    public override async UniTask StartProductionAsync()
    {
        await CalculateProductionOnLoad();
    }
    public override async UniTask CalculateProductionOnLoad()
    {
        if (_lastSavedTime != default)
        {
            TimeSpan timeDifference = DateTime.Now - _lastSavedTime;

            int maxProducedResources = (int)(timeDifference.TotalSeconds / ProductionTime);
            int producedResources = Mathf.Min(_queueCount.Value, maxProducedResources);

            var remainingResources = (timeDifference.TotalSeconds % ProductionTime);
            
            if (StoredResources.Value < MaxCapacity)
            {
                _queueCount.Value -= producedResources;
                Debug.Log("Queue " + producedResources + " kadar azald�");
            }
            //_queueCount.Value -= producedResources;

            //DecreaseQueueCount(producedResources);

            int newStoredResources = Mathf.Min(MaxCapacity, StoredResources.Value + producedResources);
            SetStoredResources(newStoredResources);

            if (newStoredResources < MaxCapacity)
            {

                double remainingTime = ProductionTime - remainingResources;
                _resourceCollector.SetSliderValue(ResourceType, (float)(remainingTime / ProductionTime));
                await ProduceWithSlider((float)(remainingTime)); // �retimi kald�g� yerden devam ettir
            }
            else
            {
                _resourceCollector.SetSliderValue(ResourceType, 1);
                _resourceCollector.SetProductionTimerText(ResourceType, "FULL");
            }
            SaveQueueCount(ConstantKeys.FlourQueueCountKey);
        }
    }
    public override async UniTask ProduceWithSlider(float remainingTime = 0)
    {
        // E�er zaten �retim yap�l�yorsa veya depo doluysa, metottan ��k
        if (IsProducing || StoredResources.Value >= MaxCapacity)
        {
            Debug.Log("�retim yap�lm�yor veya kapasite dolmu�.");
            return;
        }

        SetIsProducing(true);

        while (_queueCount.Value > 0 && StoredResources.Value < MaxCapacity)
        {
            float currentRemainingTime = remainingTime > 0 ? remainingTime : ProductionTime;

            while (currentRemainingTime > 0)
            {
                float targetValue = (float)currentRemainingTime / ProductionTime;
                _resourceCollector.SetProductionTimerText(ResourceType, Mathf.RoundToInt(currentRemainingTime) + "s");
                _resourceCollector.SetSliderValue(ResourceType, targetValue);

                currentRemainingTimeForSave = currentRemainingTime;


                await UniTask.Delay(1000); // 1 saniye bekle
                currentRemainingTime--;
            }

            SetStoredResources(StoredResources.Value + 1);//Depoya ekle
            DecreaseQueueCount();//kuyruktan ��kar

            if (IsSaveable)
            {
                SaveDatas(ConstantKeys.FlourLastSavedTimeKey, ConstantKeys.FlourStoredResourceKey);
            }

            if (_queueCount.Value <= 0)
            {
                Debug.Log("�retim kuyru�u bo�ald�, �retim durduruluyor.");
                break;
            }
        }

        _resourceCollector.SetProductionTimerText(ResourceType, StoredResources.Value >= MaxCapacity ? ConstantKeys.ProductionTimeFullText : ConstantKeys.ProductionTimeFinishText);
        _resourceCollector.SetSliderValue(ResourceType, 1);

        if (IsSaveable)
        {
            SaveDatas(ConstantKeys.FlourLastSavedTimeKey, ConstantKeys.FlourStoredResourceKey);
        }

        SetIsProducing(false);
    }
    public override async UniTask<int> CollectResources()
    {
        if (StoredResources.Value == 0) return 0;

        int collected = StoredResources.Value;

        SetStoredResources(0);//topland��� i�in s�f�rla

        if (IsSaveable)
        {
            SaveDatas(ConstantKeys.FlourLastSavedTimeKey, ConstantKeys.FlourStoredResourceKey, currentRemainingTimeForSave);
        }
        _resourceManager.AddResource(ResourceType, collected);

        if (!IsProducing)
        {
            await ProduceWithSlider();
        }
        return collected;
    }
    public async UniTask AddToQueue(ResourceType type, int quantity)
    {
        _resourceManager.RemoveResource(type, quantity);
        IncreaseQueueCount();

        if (_queueCount.Value <= 0 || IsProducing) return;

        if (IsSaveable)
        {
            if (_lastSavedTime == default)
            {
                Debug.Log("Kaydedilen zaman bulunamad�. Yeni �retim ba�lat�l�yor.");
                SaveDatas(ConstantKeys.FlourLastSavedTimeKey, ConstantKeys.FlourStoredResourceKey);
                await ProduceWithSlider();
            }
            else
            {
                await CalculateProductionOnLoad();
            }

            SaveDatas(ConstantKeys.FlourLastSavedTimeKey, ConstantKeys.FlourStoredResourceKey);
        }
        else
        {
            await ProduceWithSlider();
        }
    }
    public void RemoveFromQueue(ResourceType type, int quantity)
    {
        if (_queueCount.Value <= 0) return;
        _resourceManager.AddResource(type, quantity);
        DecreaseQueueCount();     
    }





    public override void SetSubscribes()
    {
        base.SetSubscribes();
        StoredResources.Subscribe(_ => SetSliderActive());
        QueueCount.Subscribe(_ => SetSliderActive());
    }

    public void IncreaseQueueCount()
    {
        _queueCount.Value ++;
        SaveQueueCount(ConstantKeys.FlourQueueCountKey);

    }
    public void DecreaseQueueCount()
    {
        if (_queueCount.Value > 0)
        {
            _queueCount.Value--;
        }
        SaveQueueCount(ConstantKeys.FlourQueueCountKey);
    }
    protected void SetSliderActive()
    {
        bool active = !(!IsProducing && StoredResources.Value <= 0 && QueueCount.Value <= 0);
        _resourceCollector.UpdateSliderSetActive(ResourceType, active);
    }

    protected override void SaveDatas(string lastSavedKey, string storedKey, float curr = 0)
    {
        base.SaveDatas(lastSavedKey, storedKey, curr);
        SaveQueueCount(ConstantKeys.FlourQueueCountKey);    
    }

    private void LoadQueueCount(string queueKey)
    {
        int productResources = PlayerPrefsHelper.LoadInt(queueKey);
        //Debug.Log("Loaded Queue count: " + productResources);
        _queueCount.Value = productResources;
    }
    private void SaveQueueCount(string queueKey)
    {
        PlayerPrefsHelper.SaveInt(queueKey, QueueCount.Value);
        //Debug.Log("queue count saved:" + QueueCount.Value);
    }

}
