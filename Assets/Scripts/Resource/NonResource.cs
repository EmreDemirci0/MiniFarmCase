using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Zenject;
public abstract class NonResource : ResourceBase
{
    [Inject]
    public NonResource(ResourceManager resourceManager)
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

            if (_lastSavedTime == default)
            {
                SaveDatas(_lastSavedTimeKey, _storedResourceKey);
            }
        }

        await StartProductionAsync();
    }
    public override async UniTask StartProductionAsync()
    {
        if (IsSaveable)
        {
            await CalculateProductionOnLoad();
        }
        else
        {
            await ProduceWithSlider();
        }
    }
    public override async UniTask CalculateProductionOnLoad()
    {
        if (_lastSavedTime != default)
        {
            TimeSpan timeDifference = DateTime.Now - _lastSavedTime;

            int producedResources = (int)(timeDifference.TotalSeconds / ProductionTime);
            var remainingResources = (timeDifference.TotalSeconds % ProductionTime);

            int newStoredResources = Mathf.Min(MaxCapacity, StoredResources.Value + producedResources);
            SetStoredResources(newStoredResources);

            if (newStoredResources < MaxCapacity)
            {

                double remainingTime = ProductionTime - remainingResources;
                _resourceManager.SetSliderValue(ResourceType, (float)(remainingTime / ProductionTime));
                await ProduceWithSlider((float)(remainingTime));// Resume production from where it left off
            }
            else
            {
                _resourceManager.SetSliderValue(ResourceType, 1);
                _resourceManager.SetProductionTimerText(ResourceType, "FULL");
            }
        }
    }
    public override async UniTask ProduceWithSlider(float remainingTime = 0)
    {
        if (IsProducing || StoredResources.Value >= MaxCapacity)
        {
            return;
        }

        SetIsProducing(true);

        while (StoredResources.Value < MaxCapacity)
        {
            float currentRemainingTime = remainingTime > 0 ? remainingTime : ProductionTime;
            remainingTime = 0;
            while (currentRemainingTime > 0)
            {
                float targetValue = (float)(currentRemainingTime / ProductionTime);
                _resourceManager.SetProductionTimerText(ResourceType, Mathf.RoundToInt(currentRemainingTime) + "s");
                _resourceManager.SetSliderValue(ResourceType, targetValue);

                currentRemainingTimeForSave = currentRemainingTime;

                await UniTask.Delay(1000);
                currentRemainingTime--;
            }

            SetStoredResources(StoredResources.Value + 1);//Add to Stored Resources

            if (IsSaveable)
            {
                SaveDatas(_lastSavedTimeKey, _storedResourceKey);
            }
        }

        _resourceManager.SetProductionTimerText(ResourceType, ConstantKeys.ProductionTimeFullText);
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

        SetStoredResources(0); //Reset for addition

        _resourceManager.AddResource(ResourceType, collected);

        if (IsSaveable)
        {
            SaveDatas(_lastSavedTimeKey, _storedResourceKey, currentRemainingTimeForSave);
        }

        if (!IsProducing)
        {
            await ProduceWithSlider();
        }
        return collected;
    } 
}
