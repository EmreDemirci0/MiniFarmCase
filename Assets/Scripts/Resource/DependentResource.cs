using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
public class DependentResource : ResourceBase
{
    private ReactiveProperty<int> _queueCount = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> QueueCount => _queueCount;
    protected string _queueCountKey;
    public DependentResource(ResourceManager resourceManager)
      : base(resourceManager)
    {
        InitializeAsync().Forget();
    }
    public override async UniTaskVoid InitializeAsync()
    {
        await UniTask.DelayFrame(1);
        if (IsSaveable)
        {
            _lastSavedTime = LoadSavedTime(_lastSavedTimeKey);
            LoadStoredResources(_storedResourceKey);
            LoadQueueCount(_queueCountKey);

            if (_lastSavedTime == default)
            {
                SaveDatas(_lastSavedTimeKey, _storedResourceKey);
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
            }

            int newStoredResources = Mathf.Min(MaxCapacity, StoredResources.Value + producedResources);

            SetStoredResources(newStoredResources);

            if (newStoredResources < MaxCapacity)
            {
                double remainingTime = ProductionTime - remainingResources;
                _resourceManager.SetSliderValue(ResourceType, (float)(remainingTime / ProductionTime));
                await ProduceWithSlider((float)(remainingTime)); // Resume production from where it left off
            }
            else
            {
                _resourceManager.SetSliderValue(ResourceType, 1);
                _resourceManager.SetProductionTimerText(ResourceType, "FULL");
            }
            SaveQueueCount(_queueCountKey);
        }
    }
    public override async UniTask ProduceWithSlider(float remainingTime = 0)
    {
        if (IsProducing || StoredResources.Value >= MaxCapacity)
        {
            return;
        }

        SetIsProducing(true);

        while (_queueCount.Value > 0 && StoredResources.Value < MaxCapacity)
        {
            float currentRemainingTime = remainingTime > 0 ? remainingTime : ProductionTime;
            remainingTime = 0;
            while (currentRemainingTime > 0)
            {
                float targetValue = (float)currentRemainingTime / ProductionTime;
                _resourceManager.SetProductionTimerText(ResourceType, Mathf.RoundToInt(currentRemainingTime) + "s");
                _resourceManager.SetSliderValue(ResourceType, targetValue);

                currentRemainingTimeForSave = currentRemainingTime;

                await UniTask.Delay(1000);
                currentRemainingTime--;
            }

            SetStoredResources(StoredResources.Value + 1);//Add to Stored Resources
            DecreaseQueueCount();//Remove from queue

            if (IsSaveable)
            {
                SaveDatas(_lastSavedTimeKey, _storedResourceKey);
            }

            if (_queueCount.Value <= 0)
            {
                break;
            }
        }

        _resourceManager.SetProductionTimerText(ResourceType, StoredResources.Value >= MaxCapacity ? ConstantKeys.ProductionTimeFullText : ConstantKeys.ProductionTimeFinishText);
        _resourceManager.SetSliderValue(ResourceType, 1);

        if (IsSaveable)
        {
            SaveDatas(_lastSavedTimeKey, _storedResourceKey);
        }

        SetIsProducing(false);
    }
    public override async UniTask<int> CollectResources()
    {
        if (StoredResources.Value == 0) return 0;

        int collected = StoredResources.Value;

        SetStoredResources(0);//Reset for addition

        if (IsSaveable)
        {
            SaveDatas(_lastSavedTimeKey, _storedResourceKey, currentRemainingTimeForSave);
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
            SaveDatas(_lastSavedTimeKey, _storedResourceKey, currentRemainingTimeForSave);

            if (_lastSavedTime == default)
            {
                await ProduceWithSlider();
            }
            else
            {
                await CalculateProductionOnLoad();
            }
        }
        else
        {
            await ProduceWithSlider();
        }
    }
    public void RemoveFromQueue(ResourceType type, int quantity)
    {
        if (_queueCount.Value <= 0 || quantity <= 0)
            return;

        if (IsSaveable)
        {
            SaveDatas(_lastSavedTimeKey, _storedResourceKey, currentRemainingTimeForSave);
        }
        _queueCount.Value--;
        _resourceManager.AddResource(type, quantity);
        SaveQueueCount(_queueCountKey);
    }
    public override void SetSubscribes()
    {
        base.SetSubscribes();
        StoredResources.Subscribe(_ => SetSliderActive());
        QueueCount.Subscribe(_ => SetSliderActive());
    }
    public void IncreaseQueueCount()
    {
        _queueCount.Value++;
        SaveQueueCount(_queueCountKey);

    }
    public void DecreaseQueueCount()
    {
        if (_queueCount.Value > 0)
        {
            _queueCount.Value--;
        }
        SaveQueueCount(_queueCountKey);
    }
    protected void SetSliderActive()
    {
        bool active = !(!IsProducing && StoredResources.Value <= 0 && QueueCount.Value <= 0);
        _resourceManager.UpdateSliderSetActive(ResourceType, active);
    }
    protected override void SaveDatas(string lastSavedKey, string storedKey, float curr = 0)
    {
        base.SaveDatas(lastSavedKey, storedKey, curr);
        SaveQueueCount(_queueCountKey);
    }
    private void LoadQueueCount(string queueKey)
    {
        int productResources = PlayerPrefsHelper.LoadInt(queueKey);
        _queueCount.Value = productResources;
    }
    private void SaveQueueCount(string queueKey)
    {
        PlayerPrefsHelper.SaveInt(queueKey, QueueCount.Value);
    }
}
