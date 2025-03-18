using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class BreadV1Resource : ResourceBase
{
    private ResourceManager _resourceManager;
    private ResourceCollector _resourceCollector;

    private ReactiveProperty<int> _queueCount = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> QueueCount => _queueCount;

    public bool IsProducing => isProducing;

    [Inject]
    public void Construct(ResourceCollector resourceCollector, ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
        _resourceCollector = resourceCollector;

        resourceType = ResourceType.BreadV1;



        StoredResources.Subscribe(_resourceCollector.SetBreadV1StoredResourcesText);
        StoredResources.Subscribe(_ => SetBreadV1SliderActive());
        QueueCount.Subscribe(_ => SetBreadV1SliderActive());
    }

    public async override UniTask Produce()
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
            BreadSliderTask()
            );
        }
        return collected;
    }

    public async UniTask BreadSliderTask()
    {
        Debug.Log("buna girdi");
        if (!isProducing)
        {
            Debug.Log("Üretim baþlamadý, slider duruyor.");
            return;
        }
        while (IsProducing)
        {
            Debug.Log("buna girdi2");
            int remainingTime = ProductionTime;
            while (remainingTime > 0 && isProducing)
            {
                Debug.Log("buna girdi3");
                float targetValue = (float)remainingTime / ProductionTime;

                _resourceCollector.SetProductionTimerText(resourceType, remainingTime + "s");
                _resourceCollector.SetSlider(resourceType, targetValue);

                await UniTask.WhenAny(
                      UniTask.Delay(1000),
                      UniTask.WaitUntil(() => !IsProducing) // Eðer üretim biterse hemen çýk
                    );
                Debug.Log("buna girdi4");
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

    public async UniTask AddToQueue(ResourceType type, int quantity)
    {
        _resourceManager.RemoveResource(type, quantity);
        IncreaseQueueCount();

        if (_queueCount.Value <= 0 || IsProducing)
            return;

        await UniTask.WhenAll(
            Produce(),
            BreadSliderTask()
        );
    }
    public void RemoveFromQueue(ResourceType type, int quantity)
    {
        if (_queueCount.Value <= 0)
            return;
        DecreaseQueueCount();
        _resourceManager.AddResource(type, quantity);
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
    private void SetBreadV1SliderActive()
    {
        Debug.Log("Girdi: "+ IsProducing + " : "+StoredResources.Value+" : " +QueueCount.Value);
        bool active = !(!IsProducing && StoredResources.Value <= 0 && QueueCount.Value <= 0);
        _resourceCollector.SetBreadV1SliderActive(active);
    }
}
