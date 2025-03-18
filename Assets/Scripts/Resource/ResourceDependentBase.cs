using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class ResourceDependentBase : ResourceBase
{
    
    private ReactiveProperty<int> _queueCount = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> QueueCount => _queueCount;

    public bool IsProducing => isProducing;

    [Inject]
    public ResourceDependentBase(ResourceManager resourceManager, ResourceCollector resourceCollector,ResourceType resourceType)
         : base(resourceManager, resourceCollector, resourceType)
    {

        StoredResources.Subscribe(_ => SetSliderActive());
        QueueCount.Subscribe(_ => SetSliderActive());
    }
    //[Inject]
    //public void Construct(ResourceCollector resourceCollector, ResourceManager resourceManager)
    //{
    //    _resourceManager = resourceManager;
    //    _resourceCollector = resourceCollector;

    //    resourceType = ResourceType.Flour;



    //    //StoredResources.Subscribe(_resourceCollector.SetFlourStoredResourcesText);
    //    StoredResources.Subscribe(stored => _resourceCollector.SetStoredResourcesText(resourceType, stored));//bu base classlara alýnabilir
       
    //}


    public override async UniTask Produce()
    {
        if (IsProducing || StoredResources.Value >= MaxCapacity)
        {
            Debug.Log("Üretim yapýlmýyor veya kapasite dolmuþ.");
            return;
        }

        isProducing = true;

        while (_queueCount.Value > 0 && storedResources.Value < maxCapacity)
        {
            for (int i = productionTime; i > 0; i--)
            {
                await UniTask.Delay(1000);
            }

            storedResources.Value++;//Depoya ekle
            DecreaseQueueCount();//Queue'den cýkar

            if (_queueCount.Value <= 0)
            {
                Debug.Log("Üretim kuyruðu boþaldý, üretim durduruluyor.");
                isProducing = false;
                return;
            }
        }
        isProducing = false;
    }
    public override async UniTask<int> CollectResources()
    {
        if (storedResources.Value == 0)
        {
            return 0;
        }

        int collected = storedResources.Value;
        storedResources.Value = 0;

        _resourceManager.AddResource(resourceType, collected);

        if (!isProducing)
        {
            await UniTask.WhenAll(
            Produce(),
            SliderTask()
            );
        }
        return collected;
    }
    public async UniTask AddToQueue(ResourceType type, int quantity)
    {
        _resourceManager.RemoveResource(type, quantity);
        IncreaseQueueCount();

        if (_queueCount.Value <= 0 || IsProducing)
            return;

        await UniTask.WhenAll(
            Produce(),
            SliderTask()
        );
    }
    public void RemoveFromQueue(ResourceType type, int quantity)
    {
        if (_queueCount.Value <= 0)
            return;
        DecreaseQueueCount();
        _resourceManager.AddResource(type, quantity);
    }

    public async UniTask SliderTask()
    {
        if (!isProducing)
        {
            Debug.Log("Üretim baþlamadý, slider duruyor.");
            return;
        }
        while (IsProducing)
        {
            int remainingTime = ProductionTime;
            while (remainingTime > 0 && isProducing)
            {
                float targetValue = (float)remainingTime / ProductionTime;

                _resourceCollector.SetProductionTimerText(resourceType, remainingTime + "s");
                _resourceCollector.SetSlider(resourceType, targetValue);

                await UniTask.WhenAny(
                      UniTask.Delay(1000),
                      UniTask.WaitUntil(() => !IsProducing) // Eðer üretim biterse hemen çýk
                );

                if (!IsProducing)
                {
                    _resourceCollector.SetSlider(resourceType, 1f);
                    if (StoredResources.Value >= MaxCapacity)
                        _resourceCollector.SetProductionTimerText(resourceType, "FULL");
                    else
                        _resourceCollector.SetProductionTimerText(resourceType, "FINISH");

                    return;
                }
                remainingTime--;

            }
        }
    }



    public void IncreaseQueueCount()
    {
        _queueCount.Value++;
    }

    public void DecreaseQueueCount()
    {
        if (_queueCount.Value > 0)
        {
            _queueCount.Value--; 
        }
    }
    private void SetSliderActive()
    {
        bool active = !(!IsProducing && StoredResources.Value <= 0 && QueueCount.Value <= 0);
        _resourceCollector.SetSliderActive(resourceType,active);
    }
}
