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
        //StoredResources.Subscribe(_ => SetSliderActive());
        //QueueCount.Subscribe(_ => SetSliderActive());
    }
    public override void SetSubscribes()
    {
        StoredResources.Subscribe(stored => _resourceCollector.SetResourceCapacityText(_resourceType, stored));
        StoredResources.Subscribe(_ => SetSliderActive());
        QueueCount.Subscribe(_ => SetSliderActive());
        SetResourceImage();
    }
    public override async UniTask Produce()
    {
        if (IsProducing || StoredResources.Value >= MaxCapacity)
        {
            Debug.Log("Üretim yapýlmýyor veya kapasite dolmuþ.");
            return;
        }

        SetIsProducing(true);

        while (_queueCount.Value > 0 && StoredResources.Value < MaxCapacity)
        {
            for (int i = ProductionTime; i > 0; i--)
            {
                await UniTask.Delay(1000);
            }
            SetStoredResources(StoredResources.Value+1);//Depoya ekle
            DecreaseQueueCount();//Queue'den cýkar

            if (_queueCount.Value <= 0)
            {
                Debug.Log("Üretim kuyruðu boþaldý, üretim durduruluyor.");
                SetIsProducing(false);
                return;
            }
        }
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

        _resourceManager.AddResource(_resourceType, collected);

        if (!IsProducing)
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
        if (!IsProducing)
        {
            Debug.Log("Üretim baþlamadý, slider duruyor.");
            return;
        }
        while (IsProducing)
        {
            int remainingTime = ProductionTime;
            while (remainingTime > 0 && IsProducing)
            {
                float targetValue = (float)remainingTime / ProductionTime;

                //_entityBase.SetProductionTimerText(remainingTime + "s");
                //_entityBase.SetSliderValue(targetValue);
                _resourceCollector.SetProductionTimerText(_resourceType, remainingTime + "s");
                _resourceCollector.SetSliderValue(_resourceType, targetValue);
                //_resourceCollector.SetProductionTimerText(resourceType, remainingTime + "s");
                //_resourceCollector.SetSlider(resourceType, targetValue);

                await UniTask.WhenAny(
                      UniTask.Delay(1000),
                      UniTask.WaitUntil(() => !IsProducing)  
                );

                if (!IsProducing)
                {
                   
                    _resourceCollector.SetSliderValue(_resourceType, 1);
                    //_entityBase.SetSliderValue(1);
                    //_resourceCollector.SetSlider(resourceType, 1f);
                    if (StoredResources.Value >= MaxCapacity)
                    { 
                        _resourceCollector.SetProductionTimerText(_resourceType, "FULL");
                    //_entityBase.SetProductionTimerText("FULL");

                    }
                    else
                    { 
                        _resourceCollector.SetProductionTimerText(_resourceType, "FINISH");
                   // _entityBase.SetProductionTimerText("FINISH");
                    }

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
    protected void SetSliderActive()
    {
        bool active = !(!IsProducing && StoredResources.Value <= 0 && QueueCount.Value <= 0);
        _resourceCollector.UpdateSliderSetActive(_resourceType, active);
        Debug.LogError("2SETSLÝDERACTÝVE:" + _resourceType + active);
    }
}
