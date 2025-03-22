using Cysharp.Threading.Tasks;
using System;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
public abstract class NonResource : ResourceBase
{

    [Inject]
    public NonResource(ResourceManager resourceManager, ResourceCollector resourceCollector)
         : base(resourceManager, resourceCollector)
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
                Debug.Log("Kaydedilen zaman bulunamadý. Yeni üretim baþlatýlýyor.");
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
                _resourceCollector.SetSliderValue(ResourceType, (float)(remainingTime / ProductionTime));
                await ProduceWithSlider((float)(remainingTime)); // Üretimi kaldýgý yerden devam ettir
            }
            else
            {
                _resourceCollector.SetSliderValue(ResourceType, 1);
                _resourceCollector.SetProductionTimerText(ResourceType, "FULL");
            }
        }
    }
    public override async UniTask ProduceWithSlider(float remainingTime = 0)
    {
        // Eðer zaten üretim yapýlýyorsa veya depo doluysa, metottan çýk
        if (IsProducing || StoredResources.Value >= MaxCapacity)
        {
            Debug.Log("Üretim zaten devam ediyor veya depo dolu.");
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
                _resourceCollector.SetProductionTimerText(ResourceType, Mathf.RoundToInt(currentRemainingTime) + "s");
                _resourceCollector.SetSliderValue(ResourceType, targetValue);

                currentRemainingTimeForSave = currentRemainingTime;

                await UniTask.Delay(1000);
                currentRemainingTime--;
            }

            SetStoredResources(StoredResources.Value + 1);//Depoya ekle

            if (IsSaveable)
            {
                SaveDatas(_lastSavedTimeKey, _storedResourceKey);
            }
        }

        _resourceCollector.SetProductionTimerText(ResourceType, ConstantKeys.ProductionTimeFullText);
        _resourceCollector.SetSliderValue(ResourceType, 1);

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

        SetStoredResources(0); //toplandýðý için sýfýrla

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
