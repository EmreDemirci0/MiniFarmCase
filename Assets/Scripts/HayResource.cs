using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class HayResource : ResourceBase
{
    
    private ResourceManager _resourceManager;
    private ResourceCollector _resourceCollector;

    [Inject]
    public async void Construct(ResourceManager resourceManager, ResourceCollector resourceCollector)
    {
        _resourceManager = resourceManager;
        _resourceCollector = resourceCollector;

        resourceType = ResourceType.Hay;

        StoredResources.Subscribe(_resourceCollector.SetHayStoredResourcesText);

        await UniTask.Yield();
        await UniTask.WhenAll(
           Produce(),
           HaySliderTask()
       );
    }

    public override async UniTask Produce()
    {
        //OnStoredResourcesChanged?.Invoke(0);
        if (isProducing)
        {
            return;
        }

        isProducing = true;

        while (storedResources.Value < maxCapacity)
        {
            for (int i = productionTime; i > 0; i--)
            {
                //OnProductionTimeChanged?.Invoke(i);
                await UniTask.Delay(1000);
            }

            storedResources.Value++;
            //OnStoredResourcesChanged?.Invoke(storedResources.Value);
        }

        isProducing = false;
    }

    public override async UniTask<int> CollectResources()
    {
        if (storedResources.Value == 0)
        {
            // Eðer hiç kaynak yoksa, toplama iþlemi yapýlmasýn
            return 0;
        }

        int collected = storedResources.Value;
        Debug.Log("Collect" + collected);
        storedResources.Value = 0;
        //OnStoredResourcesChanged?.Invoke(0);

        //TotalResourcesCount.Value += collected;
        _resourceManager.AddResource(resourceType, collected);
        // Only call Produce() if it's not already running
        if (!isProducing)
        {
            await Produce(); // Start production if it's not already running
        }

        return collected;
    }

    public async UniTask HaySliderTask()
    {
        while (true)
        {

            if (StoredResources.Value >= MaxCapacity)
            {
                _resourceCollector.SetProductionTimerText(resourceType, "FULL");
                _resourceCollector.SetSlider(resourceType, 1);
            }
            else
            {
                int remainingTime = ProductionTime;

                while (remainingTime > 0)
                {
                    _resourceCollector.SetProductionTimerText(resourceType, remainingTime + "s");
                    float targetValue = (float)remainingTime / ProductionTime;

                    _resourceCollector.SetSlider(resourceType, targetValue);

                    await UniTask.Delay(1000); 
                    remainingTime--;
                }
            }
            await UniTask.Yield();
        }
    }
}
