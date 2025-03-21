using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class DependentResource : ResourceBase
{
    private ReactiveProperty<int> _queueCount = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> QueueCount => _queueCount;

    public DependentResource(ResourceManager resourceManager, ResourceCollector resourceCollector)
         : base(resourceManager, resourceCollector)
    {
    }
    public override void SetSubscribes()
    {
        StoredResources.Subscribe(stored => _resourceCollector.SetResourceCapacityText(ResourceType, stored));
        StoredResources.Subscribe(_ => SetSliderActive());
        QueueCount.Subscribe(_ => SetSliderActive());
        SetResourceImage();
    }

    //    Set
    public override async UniTask ProduceWithSlider(float remainingTimes = 0)
    {
        // Eðer zaten üretim yapýlýyorsa veya depo doluysa, metottan çýk
        if (IsProducing || StoredResources.Value >= MaxCapacity)
        {
            Debug.Log("Üretim yapýlmýyor veya kapasite dolmuþ.");
            return;
        }

        // Üretimi baþlat
        SetIsProducing(true);

        // Üretim sürecini ve slider'ý takip et
        while (_queueCount.Value > 0 && StoredResources.Value < MaxCapacity)
        {
            int remainingTime = ProductionTime;

            // Slider'ý ve geri sayýmý güncelle
            while (remainingTime > 0)
            {
                float targetValue = (float)remainingTime / ProductionTime;
                _resourceCollector.SetProductionTimerText(ResourceType, remainingTime + "s");
                _resourceCollector.SetSliderValue(ResourceType, targetValue);

                await UniTask.Delay(1000); // 1 saniye bekle
                remainingTime--;
            }

            // Üretim tamamlandý, depoya ekle ve kuyruktan çýkar
            SetStoredResources(StoredResources.Value + 1);
            DecreaseQueueCount();

            // Eðer kuyruk boþaldýysa, üretimi durdur
            if (_queueCount.Value <= 0)
            {
                Debug.Log("Üretim kuyruðu boþaldý, üretim durduruluyor.");
                break;
            }
        }

        // Üretim tamamlandý, slider'ý ve metni güncelle
        if (StoredResources.Value >= MaxCapacity)
        {
            _resourceCollector.SetProductionTimerText(ResourceType, "FULL");
        }
        else
        {
            _resourceCollector.SetProductionTimerText(ResourceType, "FINISH");
        }
        _resourceCollector.SetSliderValue(ResourceType, 1);

        // Üretimi durdur
        SetIsProducing(false);
    }
    public override async UniTask<int> CollectResources()
    {
        if (StoredResources.Value == 0)
        {
            return 0;
        }

        int collected = StoredResources.Value;
        SetStoredResources(0);

        _resourceManager.AddResource(ResourceType, collected);

        if (!IsProducing)
        {
            await UniTask.WhenAll(
            //Produce(),
            //SliderTask()
            ProduceWithSlider()
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
            //Produce(),
            //SliderTask()
            ProduceWithSlider()
        );
    }
    public void RemoveFromQueue(ResourceType type, int quantity)
    {
        if (_queueCount.Value <= 0)
            return;
        DecreaseQueueCount();
        _resourceManager.AddResource(type, quantity);
    }
    //public override async UniTask Produce()
    //{
    //    if (IsProducing || StoredResources.Value >= MaxCapacity)
    //    {
    //        Debug.Log("Üretim yapýlmýyor veya kapasite dolmuþ.");
    //        return;
    //    }

    //    SetIsProducing(true);

    //    while (_queueCount.Value > 0 && StoredResources.Value < MaxCapacity)
    //    {
    //        for (int i = ProductionTime; i > 0; i--)
    //        {
    //            await UniTask.Delay(1000);
    //        }
    //        SetStoredResources(StoredResources.Value+1);//Depoya ekle
    //        DecreaseQueueCount();//Queue'den cýkar

    //        if (_queueCount.Value <= 0)
    //        {
    //            Debug.Log("Üretim kuyruðu boþaldý, üretim durduruluyor.");
    //            SetIsProducing(false);
    //            return;
    //        }
    //    }
    //public async UniTask SliderTask()
    //{
    //    if (!IsProducing)
    //    {
    //        Debug.Log("Üretim baþlamadý, slider duruyor.");
    //        return;
    //    }
    //    while (IsProducing)
    //    {
    //        int remainingTime = ProductionTime;
    //        while (remainingTime > 0 && IsProducing)
    //        {
    //            float targetValue = (float)remainingTime / ProductionTime;

    //            _resourceCollector.SetProductionTimerText(ResourceType, remainingTime + "s");
    //            _resourceCollector.SetSliderValue(ResourceType, targetValue);

    //            await UniTask.WhenAny(
    //                  UniTask.Delay(1000),
    //                  UniTask.WaitUntil(() => !IsProducing)  
    //            );

    //            if (!IsProducing)
    //            {
                   
    //                _resourceCollector.SetSliderValue(ResourceType, 1);

    //                if (StoredResources.Value >= MaxCapacity)
    //                { 
    //                    _resourceCollector.SetProductionTimerText(ResourceType, "FULL");
    //                }
    //                else
    //                { 
    //                    _resourceCollector.SetProductionTimerText(ResourceType, "FINISH");
    //                }
    //                return;
    //            }
    //            remainingTime--;

    //        }
    //    }
    //}
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
    protected void SetSliderActive()
    {
        bool active = !(!IsProducing && StoredResources.Value <= 0 && QueueCount.Value <= 0);
        _resourceCollector.UpdateSliderSetActive(ResourceType, active);
    }
}
